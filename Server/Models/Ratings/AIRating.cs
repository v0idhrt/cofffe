using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AIcontrolComputer.Models.Ratings
{
    public interface IAIRating
    {
        void ProcessRatings(string evaluationsFilePath, string ratingsFilePath);
    }
    public class Evaluation
    {
        public string EvaluatedModel { get; set; }
        public string EvaluatedModelText { get; set; }
    }

    public class EvaluationData
    {
        public string ModelName { get; set; }
        public string UserInput { get; set; }
        public string ResponseText { get; set; }
        public List<Evaluation> Evaluations { get; set; }
        public DateTime Timestamp { get; set; }
    }
    public class AIRating : IAIRating
    {
        public List<EvaluationData> LoadEvaluations(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден", filePath);

            string jsonData = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<EvaluationData>>(jsonData);
        }

        public List<RatingView> UpdateRatings(List<EvaluationData> evaluations)
        {
            var ratingViews = new List<RatingView>();

            foreach (var evaluation in evaluations)
            {
                // Проверяем, существует ли уже RatingView для этой модели
                var existingRatingView = ratingViews.Find(rv => rv.ModelName == evaluation.ModelName);
                if (existingRatingView == null)
                {
                    existingRatingView = new RatingView { ModelName = evaluation.ModelName };
                    ratingViews.Add(existingRatingView);
                }

                foreach (var eval in evaluation.Evaluations)
                {
                    var ratedModel = existingRatingView.Rated.Find(r => r.RatedModelName == eval.EvaluatedModel);
                    if (ratedModel == null)
                    {
                        ratedModel = new AIRated(eval.EvaluatedModel);
                        existingRatingView.Rated.Add(ratedModel);
                    }

                    if (eval.EvaluatedModelText.Trim() == "+")
                    {
                        ratedModel.PlusVotes++;

                        existingRatingView.Weight += 0.1; 
                    }
                    else if (eval.EvaluatedModelText.Trim() == "-")
                    {
                        ratedModel.MinusVotes++;

                        existingRatingView.Weight -= 0.05;
                    }
                }


                existingRatingView.Rating = 0; 
                foreach (var rated in existingRatingView.Rated)
                {
                    existingRatingView.Rating += rated.CalculateRating(existingRatingView.Weight);
                }

                existingRatingView.Rating = Math.Round(existingRatingView.Rating, 3);
                existingRatingView.Weight = Math.Round(existingRatingView.Weight, 3); 
            }

            return ratingViews;
        }

        public void SaveRatingsToJson(List<RatingView> ratingViews, string filePath)
        {
            string jsonData = JsonConvert.SerializeObject(ratingViews, Formatting.Indented);
            File.WriteAllText(filePath, jsonData);
        }

        public void ProcessRatings(string evaluationsFilePath, string ratingsFilePath)
        {
            var evaluations = LoadEvaluations(evaluationsFilePath);
            var ratingViews = UpdateRatings(evaluations);
            SaveRatingsToJson(ratingViews, ratingsFilePath);
        }
    }
}
