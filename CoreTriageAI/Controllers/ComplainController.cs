using CoreTriageAI.Data;
using CoreTriageAI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoreTriageAI.Controllers;

public class ComplainController : Controller
{
    private readonly AppDbContext _db;

    public ComplainController(AppDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(Complain complain)
    {
        if (!ModelState.IsValid)
            return View(complain);

        _db.Complains.Add(complain);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Your complain has been submitted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
