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
                return $"������: {response.StatusCode} - {response}";
            }
        }
        public async Task<string> SendEvaluationRequestAnthopic(string another_AIAnswer, string userInput) //���������� ��� �������� ������� �� ������.
        {
            string url = "https://api.proxyapi.ru/anthropic/v1/messages";


            var evaluationPayload = new
            {
                model = "claude-3-haiku-20240307",
                messages = new[]
                {
                    new { role = "user", content = $"�� - ����������� �������� � �������, ������� ��������� ������ �� ������ �� ������������� � ��������. " +
                    $"���� ����������� ����������� � ������� ������� �� ������� �� ������������ �������� ��������, � ����� � ����������� ��������� � �������� �����������." +
                    $"\r\n\r\n���� ������ - ������� ����� � ������� �������, ������ �� ��� �����������. " +
                    $"��� ����������, � ������� ��� ��������� ��������:\r\n\r\n- ������: {userInput}\r\n- ������������ �����: {another_AIAnswer}" +
                    $"\r\n\r\n����������, ������, ��� ��������: �+� (���������) ��� �-� (��������), ������ �� ����, ��������� �������������� ����� ������������� ������������� �������." +
                    $" \r\n\r\n������� � �������� �������, ���������� � ������� ��� ������." +
                    $" \r\n\r\n������ ������ '+' ��� '-' "}
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
                return $"������: {response.StatusCode} - {response.ReasonPhrase} | {response}";
            }
        }
    }
}
