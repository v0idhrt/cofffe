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
    public class ChatCompletionResponse
    {
        public string id { get; set; }
        public string @object { get; set; }
        public long created { get; set; }
        public string model { get; set; }
        public Choice[] choices { get; set; }
        public Usages usage { get; set; }
        public string system_fingerprint { get; set; }
    }

    public class Choice
    {
        public int index { get; set; }
        public Message message { get; set; }
        public object logprobs { get; set; }
        public string finish_reason { get; set; }
    }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
        public object refusal { get; set; }
    }

    public class Usages
    {
        public int prompt_tokens { get; set; }
        public int completion_tokens { get; set; }
        public int total_tokens { get; set; }
    }
    public interface IOpenAIAPI
    {
        Task<string> SendChatCompletionRequestGPT(string request);
        Task<string> SendEvaluationRequestGPT(string another_AIAnswer, string userInput);
    }
    public class OpenAIAPI : IOpenAIAPI
    {
        public readonly HttpClient client = new HttpClient();
        public async Task<string> SendChatCompletionRequestGPT(string request)
        {
            string url = "https://api.proxyapi.ru/openai/v1/chat/completions";

            var payload = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                       {
                    new { role = "user", content = $"{request}" }
                },
                max_tokens = 16384
            };

            string jsonPayload = JsonConvert.SerializeObject(payload);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Config.CHATGPT_API_KEY);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var chatResponse = JsonConvert.DeserializeObject<ChatCompletionResponse>(responseBody);

                var messageContent = chatResponse.choices[0].message.content;
                return messageContent;
            } else
            {
                return $"Ошибка: {response.StatusCode} - {response.ReasonPhrase}";
            }
        }
        public async Task<string> SendEvaluationRequestGPT(string another_AIAnswer, string userInput) //ПЕРЕДЕЛАТЬ ДЛЯ ОТПРАВКИ ЗАПРОСА НА ОЦЕНКУ.
        {
            string url = "https://api.proxyapi.ru/openai/v1/chat/completions";

            var payload = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                       {
                    new { role = "user", content = $"Вы - интуитивный помощник в общении, умеющий оценивать ответы на основе их релевантности и качества. " +
                    $"Ваша компетенция заключается в анализе ответов на предмет их соответствия заданным вопросам, а также в обеспечении краткости и точности предложений." +
                    $"\r\n\r\nВаша задача - оценить ответ и вынести вердикт, исходя из его пригодности. " +
                    $"Вот информация, с которой вам предстоит работать:\r\n\r\n- Вопрос: {userInput} \r\n- Предоставлен ответ: {another_AIAnswer}" +
                    $"\r\n\r\nПожалуйста, решите, что ответить: «+» (продвигаю) или «-» (отвергаю), исходя из того, насколько представленный ответ соответствует поставленному вопросу." +
                    $" \r\n\r\nПомните о важности ясности, уместности и полноты при оценке." +
                    $" \r\n\r\nОтветь только '+' или '-' "}
                },
                max_tokens = 124
            };

            string jsonPayload = JsonConvert.SerializeObject(payload);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Config.CHATGPT_API_KEY);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var chatResponse = JsonConvert.DeserializeObject<ChatCompletionResponse>(responseBody);

                var messageContent = chatResponse.choices[0].message.content;
                return messageContent;
            }
            else
            {
                return $"Ошибка: {response.StatusCode} - {response.ReasonPhrase}";
            }
        }
    }
}
