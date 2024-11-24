using Newtonsoft.Json;
using Server;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace AIcontrolComputer.Models.ModelsAPI
{
    public interface IGigaChatAPI
    {
        Task<string> SendChatCompletionRequestGigaChat(string request);
        Task<string> SendEvaluationRequestGigaChat(string another_AIAnswer, string userInput);
    }
    public class GigaChatAPI : IGigaChatAPI
    {
        public class ChatCompletionResponse
        {
            public Choice[] choices { get; set; }
            public long created { get; set; }
            public string model { get; set; }
            public string @object { get; set; }
            public Usage usage { get; set; }
        }

        public class Choice
        {
            public Message message { get; set; }
            public int index { get; set; }
            public string finish_reason { get; set; }
        }

        public class Message
        {
            public string content { get; set; }
            public string role { get; set; }
        }

        public class Usage
        {
            public int prompt_tokens { get; set; }
            public int completion_tokens { get; set; }
            public int total_tokens { get; set; }
        }
        public async Task<string> GetAccessTokenGiga()
        {
            string url = "https://ngw.devices.sberbank.ru:9443/api/v2/oauth";
            string scope = "GIGACHAT_API_PERS";
            string rqUID = Guid.NewGuid().ToString();

            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            using (var client = new HttpClient(handler))
            {
                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("scope", scope)
            });

                client.DefaultRequestHeaders.Add("RqUID", rqUID);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Config.GIGACHAT_AUTH);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
                    return jsonResponse.access_token;
                }
                else
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    return $"Ошибка: {response.StatusCode} - {response.ReasonPhrase}\nОтвет сервера: {errorResponse}";
                }
            }
        }


        public async Task<string> SendChatCompletionRequestGigaChat(string request)
        {
            string url = "https://gigachat.devices.sberbank.ru/api/v1/chat/completions";
            string accessToken = await GetAccessTokenGiga();

            var payload = new
            {
                model = "GigaChat",
                messages = new[]
                {
                new { role = "user", content = $"{request}" }
            },
                stream = false,
                repetition_penalty = 1
            };
             
            string jsonPayload = JsonConvert.SerializeObject(payload);

            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var answer = JsonConvert.DeserializeObject<ChatCompletionResponse>(responseBody);
                    return answer.choices[0].message.content;
                }
                else
                {
                    return $"Ошибка: {response.StatusCode} - {response.ReasonPhrase}";
                }
            }
        }
        public async Task<string> SendEvaluationRequestGigaChat(string another_AIAnswer, string userInput) //ПЕРЕДЕЛАТЬ ДЛЯ ОТПРАВКИ ЗАПРОСА НА ОЦЕНКУ.
        {
            string url = "https://gigachat.devices.sberbank.ru/api/v1/chat/completions";
            string accessToken = await GetAccessTokenGiga();

            var payload = new
            {
                model = "GigaChat",
                messages = new[]
                {
                    new { role = "user", content = $"Вы - интуитивный помощник в общении, умеющий оценивать ответы на основе их релевантности и качества. " +
                    $"Ваша компетенция заключается в анализе ответов на предмет их соответствия заданным вопросам, а также в обеспечении краткости и точности предложений." +
                    $"\r\n\r\nВаша задача - оценить ответ и вынести вердикт, исходя из его пригодности. " +
                    $"Вот информация, с которой вам предстоит работать:\r\n\r\n- Вопрос: {userInput}\r\n- Предоставлен ответ: {another_AIAnswer}" +
                    $"\r\n\r\nПожалуйста, решите, что ответить: «+» (продвигаю) или «-» (отвергаю), исходя из того, насколько представленный ответ соответствует поставленному вопросу." +
                    $" \r\n\r\nПомните о важности ясности, уместности и полноты при оценке." }
            },
                stream = false,
                repetition_penalty = 1
            };

            string jsonPayload = JsonConvert.SerializeObject(payload);

            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var answer = JsonConvert.DeserializeObject<ChatCompletionResponse>(responseBody);
                    return answer.choices[0].message.content;
                }
                else
                {
                    return $"Ошибка: {response.StatusCode} - {response.ReasonPhrase}";
                }
            }
        }
    }
}
