using AIcontrolComputer.Models.AIAnswerProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AIcontrolComputer.Models.AIAnswerProcessing.ResponseProcessingModule;

namespace AIcontrolComputer.Utils
{
    public interface IDisplay
    {
        Task DisplayAllAnswers(string request);
        void DisplayWaitingMessaga(string modelName);
        Task DisplayAllEvaluations(Task<Dictionary<string, EvaluationResult>> taskEvaluations);
    }

    public class Display : IDisplay
    {
        IResponseProcessingModule responseProcessingModule = new ResponseProcessingModule();
        public async Task DisplayAllAnswers(string request)
        {
            List<AIResponse> responses = await responseProcessingModule.GetAllAnswer(request);
            Console.WriteLine("Ответы от AI:");

            foreach (var response in responses)
            {
                Console.WriteLine($"Модель: {response.ModelName}");
                Console.WriteLine($"Ответ: {response.ResponseText}");
                Console.WriteLine(new string('-', 50));
            }
        }



        public async Task DisplayAllEvaluations(Task<Dictionary<string, EvaluationResult>> taskEvaluations)
        {
            Dictionary<string, EvaluationResult> evaluations = await taskEvaluations;

            foreach (var entry in evaluations)
            {
                string modelName = entry.Key;
                EvaluationResult evaluationResult = entry.Value;

                if (evaluationResult.Evaluations.Count > 0)
                {
                    Console.WriteLine($"Модель AI: {modelName}");
                    Console.WriteLine($"Введенный пользователем текст: {evaluationResult.UserInput}");
                    Console.WriteLine($"Оцениваемый текст: {evaluationResult.ResponseText}");
                    Console.WriteLine("Оценки:");

                    foreach (var evaluation in evaluationResult.Evaluations)
                    {
                        Console.WriteLine($"- Ответ от {evaluation.EvaluatedModel}: {evaluation.EvaluatedModelText}");
                    }

                    Console.WriteLine(new string('*', 60));
                }
                else
                {
                    Console.WriteLine($"Модель AI: {modelName} не имеет оценок.");
                    Console.WriteLine(new string('*', 60));
                }
            }
        }


        public void DisplayWaitingMessaga(string modelName)
        {
            Console.WriteLine($"Ожидается ответ от {modelName}");
        }
    }
}