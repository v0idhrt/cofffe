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

    public class ApiResponse
    {
        public Result result { get; set; }
    }

    public class Result
    {
        public Alternative[] alternatives { get; set; }
        public Usage_ usage { get; set; }
        public string modelVersion { get; set; }
    }

    public class Alternative
    {
        public Messages message { get; set; }
        public string status { get; set; }
    }

    public class Messages
    {
        public string role { get; set; }
        public string text { get; set; }
    }

    public class Usage_
    {
        public string inputTextTokens { get; set; }
        public string completionTokens { get; set; }
        public string totalTokens { get; set; }
    }
    public interface IYandexAPI
    {
        Task<string> SendChatCompletionRequestYandex(string request);
        Task<string> SendEvaluationRequestYandex(string another_AIAnswer, string userInput);
    }

    public class YandexAIAPI : IYandexAPI
    {
        public readonly HttpClient client = new HttpClient();

        public async Task<string> SendChatCompletionRequestYandex(string request)
        {
            string url = "https://llm.api.cloud.yandex.net/foundationModels/v1/completion";

            var payload = new
            {
                modelUri = "gpt://b1g8s5iuboiim57cp6gi/yandexgpt-lite",
                completionOptions = new
                {
                    stream = false,
                    temperature = 0.6,
                    maxTokens = "2000"
                },
                messages = new[]
                {
                new { role = "system", text = "Ты языковая модель, отвечающая на вопросы и старающаяся помочь пользовотелю везде, где можно" },
                new { role = "user", text = $"{request}" },
                }
            };


            string jsonPayload = JsonConvert.SerializeObject(payload);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Api-Key", Config.YAN_API_KEY);

            var content = new StringContent(jsonPayload);

            HttpResponseMessage response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var chatResponse = JsonConvert.DeserializeObject<ApiResponse>(responseBody);

                var messageContent = chatResponse.result.alternatives[0].message.text;

                return messageContent;
            } else
            {
                Console.WriteLine(response);
                return $"Ошибка: {response.StatusCode} - {response.ReasonPhrase}";
            }
        }
        public async Task<string> SendEvaluationRequestYandex(string another_AIAnswer, string userInput) //ПЕРЕДЕЛАТЬ ДЛЯ ОТПРАВКИ ЗАПРОСА НА ОЦЕНКУ.
        {
            string url = "https://llm.api.cloud.yandex.net/foundationModels/v1/completion";

            var payload = new
            {
                modelUri = "gpt://b1g8s5iuboiim57cp6gi/yandexgpt-lite",
                completionOptions = new
                {
                    stream = false,
                    temperature = 0.6,
                    maxTokens = "2000"
                },
                messages = new[]
                {
                new { role = "system", text = "Ты языковая модель, отвечающая на вопросы и старающаяся помочь пользовотелю везде, где можно" },
                new { role = "user", text = $"Вы - интуитивный помощник в общении, умеющий оценивать ответы на основе их релевантности и качества. " +
                    $"Ваша компетенция заключается в анализе ответов на предмет их соответствия заданным вопросам, а также в обеспечении краткости и точности предложений." +
                    $"\r\n\r\nВаша задача - оценить ответ и вынести вердикт, исходя из его пригодности. " +
                    $"Вот информация, с которой вам предстоит работать:\r\n\r\n- Вопрос: {userInput} \r\n- Предоставлен ответ: {another_AIAnswer}" +
                    $"\r\n\r\nПожалуйста, решите, что ответить: «+» (продвигаю) или «-» (отвергаю), исходя из того, насколько представленный ответ соответствует поставленному вопросу." +
                    $" \r\n\r\nПомните о важности ясности, уместности и полноты при оценке." +
                    $" \r\n\r\nОтветь только '+' или '-' "}
                }
            };


            string jsonPayload = JsonConvert.SerializeObject(payload);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Api-Key", Config.YAN_API_KEY);

            var content = new StringContent(jsonPayload);

            HttpResponseMessage response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var chatResponse = JsonConvert.DeserializeObject<ApiResponse>(responseBody);

                var messageContent = chatResponse.result.alternatives[0].message.text;

                return messageContent;
            }
            else
            {
                Console.WriteLine(response);
                return $"Ошибка: {response.StatusCode} - {response.ReasonPhrase}";
            }
        }
    }
}
