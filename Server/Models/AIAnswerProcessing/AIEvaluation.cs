using AIcontrolComputer.Models.AIAnswerProcessing;
using System;
public class AIEvaluation : AIResponse
{
    public string EvaluatedModel { get; set; }
    public string EvaluatedModelText { get; set; }

    public AIEvaluation(string modelName, string responseText, string evaluatedModel, string evaluatedModelText)  : base(modelName, responseText)
    {
        EvaluatedModel = evaluatedModel;
        EvaluatedModelText = evaluatedModelText;
    }
}