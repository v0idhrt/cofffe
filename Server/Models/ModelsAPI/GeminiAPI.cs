using Newtonsoft.Json;
using Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AIcontrolComputer.Models.ModelsAPI
{
    public interface IGeminiAPI
    {
        Task<string> SendChatCompletionRequestGemini(string request);
        Task<string> SendEvaluationRequestGemini(string another_AIAnswer, string userInput);
    }

    public class Response
    {
        public Candidate[] candidates { get; set; }
        public UsageMetadata usageMetadata { get; set; }
    }

    public class Candidate
    {
        public Content content { get; set; }
        public string finishReason { get; set; }
        public int index { get; set; }
        public SafetyRating[] safetyRatings { get; set; }
    }

    public class Content
    {
        public Part[] parts { get; set; }
        public string role { get; set; }
    }

    public class Part
    {
        public string text { get; set; }
    }

    public class SafetyRating
    {
        public string category { get; set; }
        public string probability { get; set; }
    }

    public class UsageMetadata
    {
        public int promptTokenCount { get; set; }
        public int candidatesTokenCount { get; set; }
        public int totalTokenCount { get; set; }
    }

    public class GeminiAPI : IGeminiAPI
    {

        public readonly HttpClient client = new HttpClient();
        public async Task<string> SendChatCompletionRequestGemini(string request)
        {

            string url = "https://api.proxyapi.ru/google/v1/models/gemini-1.5-pro:generateContent";

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new { text = $"{request}" }
                        }
                    }
                },
            };

            string jsonPayload = JsonConvert.SerializeObject(payload);


            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Config.API_GEM_KEY);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode) {
                string responseBody = await response.Content.ReadAsStringAsync();
                var chatResponse = JsonConvert.DeserializeObject<Response>(responseBody);

                var messageContent = chatResponse.candidates[0].content.parts[0].text;
                return messageContent.Trim();
            } else
            {
                return $"Ошибка: {response.StatusCode} - {response.ReasonPhrase}";
            }
        }
        public async Task<string> SendEvaluationRequestGemini(string another_AIAnswer, string userInput) //ПЕРЕДЕЛАТЬ ДЛЯ ОТПРАВКИ ЗАПРОСА НА ОЦЕНКУ.
        { 

            string url = "https://api.proxyapi.ru/google/v1/models/gemini-1.5-pro:generateContent";

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new {text = $"Вы - интуитивный помощник в общении, умеющий оценивать ответы на основе их релевантности и качества. " +
                            $"Ваша компетенция заключается в анализе ответов на предмет их соответствия заданным вопросам, а также в обеспечении краткости и точности предложений." +
                            $"\r\n\r\nВаша задача - оценить ответ и вынести вердикт, исходя из его пригодности. " +
                            $"Вот информация, с которой вам предстоит работать:\r\n\r\n- Вопрос: {userInput}\r\n- Предоставлен ответ: {another_AIAnswer}" +
                            $"\r\n\r\nПожалуйста, решите, что ответить: «+» (продвигаю) или «-» (отвергаю), исходя из того, насколько представленный ответ соответствует поставленному вопросу." +
                            $" \r\n\r\nПомните о важности ясности, уместности и полноты при оценке." +
                            $" \r\n\r\nОтветь только '+' или '-' "}
                        }
                    }
                },
            };

            string jsonPayload = JsonConvert.SerializeObject(payload);


            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Config.API_GEM_KEY);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var chatResponse = JsonConvert.DeserializeObject<Response>(responseBody);

                var messageContent = chatResponse.candidates[0].content.parts[0].text;
                return messageContent.Trim();
            }
            else
            {
                return $"Ошибка: {response.StatusCode} - {response.ReasonPhrase}";
            }
        }
    }
}
