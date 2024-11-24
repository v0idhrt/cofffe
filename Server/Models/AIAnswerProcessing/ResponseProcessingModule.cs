using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using AIcontrolComputer.Models.ModelsAPI;
using AIcontrolComputer.Utils;
using static AIcontrolComputer.Utils.Display;

namespace AIcontrolComputer.Models.AIAnswerProcessing
{
    public interface IResponseProcessingModule
    {
        Task<List<AIResponse>> GetAllAnswer(string request);
        Task<Dictionary<string, EvaluationResult>> GetAllAIEvaluation(Task<List<AIResponse>> answersToEvaluateTask,  string userInput);
        Task<string> GetClaudAnswer(string request);
        Task<string> GetGeminiAnswer(string request);
        Task<string> GetYandexAnswer(string request);
        Task<string> GetGptAnswer(string request);
        Task<string> GetGigaChatAnswer(string request);
        Task<string> GetMistralAnswer(string request);
    }
    public class ResponseProcessingModule : IResponseProcessingModule
    {


        public async Task<List<AIResponse>> GetAllAnswer(string request)
        {
            List<AIResponse> responses = new List<AIResponse>();
            IDisplay waitingStatus = new Display();

            IAnthopicAPI anthopicClient = new AnthopicAPI();
            IGeminiAPI geminiClient = new GeminiAPI();
            IYandexAPI yandexClient = new YandexAIAPI();
            IOpenAIAPI openaiClient = new OpenAIAPI();
            IGigaChatAPI gigachatClient = new GigaChatAPI();
            IMistralAIAPI mistralaiClient = new MistralAIAPI();

            var tasks = new List<Task<AIResponse>>();

            waitingStatus.DisplayWaitingMessaga("Claud");
            tasks.Add(GetResponseAsync("Claud", request, anthopicClient.SendChatCompletionRequestAnthopic));

            waitingStatus.DisplayWaitingMessaga("Gemini");
            tasks.Add(GetResponseAsync("Gemini", request, geminiClient.SendChatCompletionRequestGemini));

            waitingStatus.DisplayWaitingMessaga("YandexGPT");
            tasks.Add(GetResponseAsync("YandexGPT", request, yandexClient.SendChatCompletionRequestYandex));

            waitingStatus.DisplayWaitingMessaga("GPT");
            tasks.Add(GetResponseAsync("GPT", request, openaiClient.SendChatCompletionRequestGPT));

            waitingStatus.DisplayWaitingMessaga("GigaChat");
            tasks.Add(GetResponseAsync("GigaChat", request, gigachatClient.SendChatCompletionRequestGigaChat));

            waitingStatus.DisplayWaitingMessaga("Mistral");
            tasks.Add(GetResponseAsync("Mistral", request, mistralaiClient.SendChatCompletionRequestMistral));

            responses.AddRange(await Task.WhenAll(tasks));

            waitingStatus.DisplayWaitingMessaga("Все ответы получены!");

            return responses;
        }


        public async Task<string> GetClaudAnswer(string request)
        {
            IAnthopicAPI an = new AnthopicAPI();
            string answer = await an.SendChatCompletionRequestAnthopic(request);
            return answer;
        }

        public async Task<string> GetGeminiAnswer(string request)
        {
            IGeminiAPI an = new GeminiAPI();
            string answer = await an.SendChatCompletionRequestGemini(request);
            return answer;
        }

        public async Task<string> GetYandexAnswer(string request)
        {
            IYandexAPI an = new YandexAIAPI();
            string answer = await an.SendChatCompletionRequestYandex(request);
            return answer;
        }

        public async Task<string> GetMistralAnswer(string request)
        {
            IMistralAIAPI an = new MistralAIAPI();
            string answer = await an.SendChatCompletionRequestMistral(request);
            return answer;
        }


        public async Task<string> GetGptAnswer(string request)
        {
            IOpenAIAPI an = new OpenAIAPI();
            string answer = await an.SendChatCompletionRequestGPT(request);
            return answer;
        }


        public async Task<string> GetGigaChatAnswer(string request)
        {
            IGigaChatAPI an = new GigaChatAPI();
            string answer = await an.SendChatCompletionRequestGigaChat(request);
            return answer;
        }

        private async Task<AIResponse> GetResponseAsync(string modelName, string request, Func<string, Task<string>> apiCall)
        {
            string responseText = await apiCall(request);
            return new AIResponse(modelName, responseText);
        }

        public async Task<AIResponse> GetAIEvaluation(string AI_answer, string AI_Model, string userInput)
        {
            string response_text = String.Empty;
            switch (AI_Model)
            {
                case "Claud":
                    IAnthopicAPI anthopicClient = new AnthopicAPI();
                    response_text = await anthopicClient.SendEvaluationRequestAnthopic(AI_answer, userInput);
                    break;
                case "Gemini":
                    IGeminiAPI geminiClient = new GeminiAPI();
                    response_text = await geminiClient.SendEvaluationRequestGemini(AI_answer, userInput);
                    break;
                case "GPT":
                    IOpenAIAPI openaiClient = new OpenAIAPI();
                    response_text = await openaiClient.SendEvaluationRequestGPT(AI_answer, userInput);
                    break;
                case "YandexGPT":
                    IYandexAPI yandexClient = new YandexAIAPI();
                    response_text = await yandexClient.SendEvaluationRequestYandex(AI_answer, userInput);
                    break;
                case "GigaChat":
                    IGigaChatAPI gigachatClient = new GigaChatAPI();
                    response_text = await gigachatClient.SendEvaluationRequestGigaChat(AI_answer, userInput);
                    break;
                case "Mistral":
                    IMistralAIAPI mistralClient = new MistralAIAPI();
                    response_text = await mistralClient.SendEvaluationRequestMistral(AI_answer, userInput);
                    break;

            }
            return new AIResponse(AI_Model, response_text);
        }

        public async Task<Dictionary<string, EvaluationResult>> GetAllAIEvaluation(Task<List<AIResponse>> answersToEvaluateTask, string userInput)
        {
            IDisplay display = new Display();

            List<string> allAi = new List<string> { "Claud", "GigaChat", "YandexGPT", "GPT", "Gemini", "Mistral" }; 
            Dictionary<string, EvaluationResult> dictOfModelPlusEvaluation = new Dictionary<string, EvaluationResult>();

            List<AIResponse> answersToEvaluate = await answersToEvaluateTask;

            foreach (var answer in answersToEvaluate)
            {
                EvaluationResult evaluationResult = new EvaluationResult
                {
                    ModelName = answer.ModelName,
                    UserInput = userInput,
                    ResponseText = answer.ResponseText
                };
                Console.WriteLine(new string('-', 60));
                foreach (string ai in allAi)
                {
                    if (ai != answer.ModelName)
                    {
                        display.DisplayWaitingMessaga(ai);
                        AIResponse evaluationAnswer = await GetAIEvaluation(answer.ResponseText, ai, userInput);
                        evaluationResult.Evaluations.Add(new Evaluation(ai, evaluationAnswer.ResponseText));
                    }
                }
                dictOfModelPlusEvaluation.Add(answer.ModelName, evaluationResult);
            }
            
            AnswersSaver saver = new AnswersSaver();
            saver.SaveAnswersAndEvaluations(dictOfModelPlusEvaluation.Values.ToList());

            return dictOfModelPlusEvaluation;
        }

    }
}
