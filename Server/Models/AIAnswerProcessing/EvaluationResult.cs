public class EvaluationResult
{
    public string ModelName { get; set; }

    public string UserInput { get; set; }
    public string ResponseText { get; set; }
    public List<Evaluation> Evaluations { get; set; }
    public DateTime Timestamp { get; set; }

    public EvaluationResult()
    {
        Evaluations = new List<Evaluation>();
        Timestamp = DateTime.Now;
    }
}

public class Evaluation
{
    public string EvaluatedModel { get; set; }
    public string EvaluatedModelText { get; set; }

    public Evaluation(string evaluatedModel, string evaluatedModelText)
    {
        EvaluatedModel = evaluatedModel;
        EvaluatedModelText = evaluatedModelText;
    }
}
