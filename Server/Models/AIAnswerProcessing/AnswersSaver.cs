using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class AnswersSaver
{
    public void SaveAnswersAndEvaluations(List<EvaluationResult> results)
    {
        List<EvaluationResult> existingResults = new List<EvaluationResult>();
        string dataFolderPath = Path.Combine(Environment.CurrentDirectory, "Data");

        if (!Directory.Exists(dataFolderPath))
        {
            Directory.CreateDirectory(dataFolderPath);
        }

        string filePath = Path.Combine(dataFolderPath, "answers_and_evaluations.json");

        if (File.Exists(filePath))
        {
            try
            {
                string existingData = File.ReadAllText(filePath);
                existingResults = JsonConvert.DeserializeObject<List<EvaluationResult>>(existingData) ?? new List<EvaluationResult>();
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"Ошибка при чтении файла JSON: {jsonEx.Message}");
                return;
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"Ошибка при доступе к файлу: {ioEx.Message}");
                return;
            }
        }

        foreach (var result in existingResults)
        {
            result.Timestamp = DateTime.Now;
        }


        existingResults.AddRange(results);

        string jsonData = JsonConvert.SerializeObject(existingResults, Formatting.Indented);

        try
        {
            File.WriteAllText(filePath, jsonData);
        }
        catch (IOException ioEx)
        {
            Console.WriteLine($"Ошибка при записи в файл: {ioEx.Message}");
        }
    }
}
