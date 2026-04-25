namespace CoreTriageAI.Models;

public class Complain
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Open";
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public string? AssignedTo { get; set; }
}
