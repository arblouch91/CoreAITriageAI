namespace CoreTriageAI.Models;

public class ComplainListViewModel
{
    public List<Complain> Complains { get; set; } = [];
    public int TotalCount { get; set; }
    public int HighPriorityCount { get; set; }
    public decimal AvgSentiment { get; set; }
    public int FilteredCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public string? SortBy { get; set; }
    public string? Department { get; set; }
    public string? Priority { get; set; }
    public string? Category { get; set; }
    public string? Status { get; set; }
}
