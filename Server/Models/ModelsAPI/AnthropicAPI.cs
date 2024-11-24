using Newtonsoft.Json;
using Server;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;



namespace AIcontrolComputer.Models.ModelsAPI
{
    public class MessageResponse
    {
        public string id { get; set; }
        public string type { get; set; }
        public string role { get; set; }
        public string model { get; set; }
        public Contents[] content { get; set; }
        public string stop_reason { get; set; }
        public object stop_sequence { get; set; }
        public Usage usage { get; set; }
    }

    public class Contents
    {
        public string type { get; set; }
        public string text { get; set; }
    }

    public class Usage
    {
        public int input_tokens { get; set; }
        public int output_tokens { get; set; }
    }
    public interface IAnthopicAPI
    {
        Task<string> SendChatCompletionRequestAnthopic(string request);
        Task<string> SendEvaluationRequestAnthopic(string another_AIAnswer, string userInput);
    }

    public class AnthopicAPI : IAnthopicAPI
    {
        public readonly HttpClient client = new HttpClient();

        public async Task<string> SendChatCompletionRequestAnthopic(string request)
        {
            string url = "https://api.proxyapi.ru/anthropic/v1/messages";


            var requestPayload = new
            {
                model = "claude-3-haiku-20240307",
                messages = new[]
                {
                new { role = "user", content = $"{request}" }
            },
                max_tokens = 4096
            };

            string jsonPayload = JsonConvert.SerializeObject(requestPayload);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Config.API_KEY_CLAUD);
            client.DefaultRequestHeaders.Add("Anthropic-Version", "2023-06-01");

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");



            HttpResponseMessage response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var chatResponse = JsonConvert.DeserializeObject<MessageResponse>(responseBody);

                var messageContent = chatResponse.content[0].text;
                return messageContent;
            }
            else
            {
                return $"Ошибка: {response.StatusCode} - {response}";
            }
        }
        public async Task<string> SendEvaluationRequestAnthopic(string another_AIAnswer, string userInput) //ПЕРЕДЕЛАТЬ ДЛЯ ОТПРАВКИ ЗАПРОСА НА ОЦЕНКУ.
        {
            string url = "https://api.proxyapi.ru/anthropic/v1/messages";


            var evaluationPayload = new
            {
                model = "claude-3-haiku-20240307",
                messages = new[]
                {
                    new { role = "user", content = $"Вы - интуитивный помощник в общении, умеющий оценивать ответы на основе их релевантности и качества. " +
                    $"Ваша компетенция заключается в анализе ответов на предмет их соответствия заданным вопросам, а также в обеспечении краткости и точности предложений." +
                    $"\r\n\r\nВаша задача - оценить ответ и вынести вердикт, исходя из его пригодности. " +
                    $"Вот информация, с которой вам предстоит работать:\r\n\r\n- Вопрос: {userInput}\r\n- Предоставлен ответ: {another_AIAnswer}" +
                    $"\r\n\r\nПожалуйста, решите, что ответить: «+» (продвигаю) или «-» (отвергаю), исходя из того, насколько представленный ответ соответствует поставленному вопросу." +
                    $" \r\n\r\nПомните о важности ясности, уместности и полноты при оценке." +
                    $" \r\n\r\nОтветь только '+' или '-' "}
                },
                max_tokens = 4096
            };

            string jsonPayload = JsonConvert.SerializeObject(evaluationPayload);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Config.API_KEY_CLAUD);
            client.DefaultRequestHeaders.Add("Anthropic-Version", "2023-06-01");

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");



            HttpResponseMessage response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var chatResponse = JsonConvert.DeserializeObject<MessageResponse>(responseBody);

                var messageContent = chatResponse.content[0].text;
                return messageContent;
            }
            else
            {
                return $"Ошибка: {response.StatusCode} - {response.ReasonPhrase} | {response}";
            }
        }
    }
}
