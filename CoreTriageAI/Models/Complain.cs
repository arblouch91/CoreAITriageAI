namespace CoreTriageAI.Models;

public class Complain
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string ComplainText { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Priority { get; set; } = "Medium";
    public int? SentimentScore { get; set; }
    public string? SentimentLabel { get; set; }
}
