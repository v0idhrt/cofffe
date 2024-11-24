public class RatingView
{
    public string ModelName { get; set; }
    public double Weight { get; set;  }
    public List<AIRated> Rated { get; set; }
    public double Rating { get; set; }
    public RatingView()
    {
        Rated = new List<AIRated>();
        Weight = 1.0;
        Rating = 0.0;
    }
}
public class AIRated
{
    public string RatedModelName { get; set; }
    public int PlusVotes { get; set; }
    public int MinusVotes { get; set; }
    public AIRated(string ratedModelName)
    {
        RatedModelName = ratedModelName;
        PlusVotes = 0;
        MinusVotes = 0;
    }
    public double CalculateRating(double weight)
    {
        return (PlusVotes * weight) - (MinusVotes * weight);
    }
}