using CoreTriageAI.Data;
using CoreTriageAI.Models;
using CoreTriageAI.Services;
using Microsoft.AspNetCore.Mvc;

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
