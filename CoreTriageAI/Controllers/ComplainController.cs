using CoreTriageAI.Data;
using CoreTriageAI.Models;
using CoreTriageAI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreTriageAI.Controllers;

public class ComplainController : Controller
{
    private readonly AppDbContext _db;
    private readonly OpenRouterService _llm;

    public ComplainController(AppDbContext db, OpenRouterService llm)
    {
        _db = db;
        _llm = llm;
    }

    public IActionResult Index() => View();

    public async Task<IActionResult> List(
        int page = 1,
        string? sortBy = null,
        string? department = null,
        string? priority = null)
    {
        const int pageSize = 10;

        var totalCount = await _db.Complains.CountAsync();
        var highPriorityCount = await _db.Complains.CountAsync(c => c.Priority == "high");
        var avgSentiment = await _db.Complains.AverageAsync(c => (decimal?)c.SentimentScore) ?? 0m;

        var query = _db.Complains.AsQueryable();

        if (!string.IsNullOrEmpty(department))
            query = query.Where(c => c.Department == department);

        if (!string.IsNullOrEmpty(priority))
            query = query.Where(c => c.Priority == priority);

        var filteredCount = await query.CountAsync();

        query = sortBy switch
        {
            "date_asc"      => query.OrderBy(c => c.CreatedAt),
            "priority_desc" => query.OrderByDescending(c => c.Priority == "low" ? 1 : c.Priority == "medium" ? 2 : 3),
            "priority_asc"  => query.OrderBy(c => c.Priority == "low" ? 1 : c.Priority == "medium" ? 2 : 3),
            _               => query.OrderByDescending(c => c.CreatedAt)
        };

        var totalPages = (int)Math.Ceiling(filteredCount / (double)pageSize);
        page = Math.Max(1, Math.Min(page, totalPages > 0 ? totalPages : 1));

        var complains = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return View(new ComplainListViewModel
        {
            Complains        = complains,
            TotalCount       = totalCount,
            HighPriorityCount = highPriorityCount,
            AvgSentiment     = avgSentiment,
            FilteredCount    = filteredCount,
            CurrentPage      = page,
            PageSize         = pageSize,
            TotalPages       = totalPages,
            SortBy           = sortBy,
            Department       = department,
            Priority         = priority
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(
        [FromForm] string Name,
        [FromForm] string Email,
        [FromForm] string PhoneNumber,
        [FromForm] string ComplainText)
    {
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(PhoneNumber) || string.IsNullOrWhiteSpace(ComplainText))
            return Json(new { success = false, error = "All fields are required." });

        try
        {
            var analysis = await _llm.AnalyzeAsync(Name, Email, ComplainText);

            _db.Complains.Add(new Complain
            {
                Name = Name,
                Email = Email,
                PhoneNumber = PhoneNumber,
                ComplainText = ComplainText,
                Department = analysis.Department,
                Priority = analysis.Priority,
                SentimentScore = analysis.SentimentsScore,
                SentimentLabel = analysis.SentimentsLabel,
                CreatedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();

            return Json(new { success = true });
        }
        catch
        {
            return Json(new { success = false, error = "We couldn't analyze your complaint right now. Please try again." });
        }
    }
}
