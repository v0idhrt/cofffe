using AIcontrolComputer.Models.AIAnswerProcessing;
using System;
public class AIResponse
{
    public string ModelName { get; set; }
    public string ResponseText { get; set; }

    public AIResponse(string modelName, string responseText)
    {
        ModelName = modelName;
        ResponseText = responseText;
    }
}