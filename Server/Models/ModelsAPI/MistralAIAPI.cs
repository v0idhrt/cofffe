using Newtonsoft.Json;
using Server;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AIcontrolComputer.Models.ModelsAPI
{
    public interface IMistralAIAPI
    {
        Task<string> SendChatCompletionRequestMistral(string request);
        Task<string> SendEvaluationRequestMistral(string another_AIAnswer, string userInput);
    }


    public class MistralAIAPI : IMistralAIAPI
    {
        public class ChatCompletionResponse
        {
            public string id { get; set; }
            public string @object { get; set; }
            public long created { get; set; }
            public string model { get; set; }
            public Choice[] choices { get; set; }
            public Usage usage { get; set; }
        }

        public class Choice
        {
            public int index { get; set; }
            public Message message { get; set; }
            public string finish_reason { get; set; }
            public object logprobs { get; set; }
        }

        public class Message
        {
            public string role { get; set; }
            public string content { get; set; }
            public object tool_calls { get; set; }
        }

        public class Usage
        {
            public int prompt_tokens { get; set; }
            public int total_tokens { get; set; }
            public int completion_tokens { get; set; }
        }


        public readonly HttpClient client = new HttpClient();

        public async Task<string> SendChatCompletionRequestMistral(string request)
        {

            string url = "https://api.mistral.ai/v1/chat/completions";

            var payload = new
            {
                model = "mistral-large-latest",
                messages = new[]
            {
                new { role = "user", content = $"{request}" }
            }
            };

            string jsonPayload = JsonConvert.SerializeObject(payload);


            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Config.MISTRAL_API_KEY);

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
                return $"������: {response.StatusCode} - {response.ReasonPhrase}";
            }
        }
        public async Task<string> SendEvaluationRequestMistral(string another_AIAnswer, string userInput) //���������� ��� �������� ������� �� ������.
        {

            string url = "https://api.mistral.ai/v1/chat/completions";

            var payload = new
            {
                model = "mistral-large-latest",
                messages = new[]
            {
                new { role = "user", content = $"�� - ����������� �������� � �������, ������� ��������� ������ �� ������ �� ������������� � ��������. " +
                    $"���� ����������� ����������� � ������� ������� �� ������� �� ������������ �������� ��������, � ����� � ����������� ��������� � �������� �����������." +
                    $"\r\n\r\n���� ������ - ������� ����� � ������� �������, ������ �� ��� �����������. " +
                    $"��� ����������, � ������� ��� ��������� ��������:\r\n\r\n- ������: {userInput} \r\n- ������������ �����: {another_AIAnswer}" +
                    $"\r\n\r\n����������, ������, ��� ��������: �+� (���������) ��� �-� (��������), ������ �� ����, ��������� �������������� ����� ������������� ������������� �������." +
                    $" \r\n\r\n������� � �������� �������, ���������� � ������� ��� ������." +
                    $" \r\n\r\n������ ������ '+' ��� '-' "}
            }
            };

            string jsonPayload = JsonConvert.SerializeObject(payload);


            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;


            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Config.MISTRAL_API_KEY);

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
                return $"������: {response.StatusCode} - {response.ReasonPhrase}";
            }
        }
    }
}
