using System.Diagnostics;
using CoreTriageAI.Data;
using CoreTriageAI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreTriageAI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _db;

        public HomeController(ILogger<HomeController> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index(
            int page = 1,
            string? sortBy = null,
            string? department = null,
            string? priority = null,
            string? category = null,
            string? status = null)
        {
            const int pageSize = 10;

            var totalCount        = await _db.Complains.CountAsync();
            var highPriorityCount = await _db.Complains.CountAsync(c => c.Priority == "high");
            var avgSentiment      = await _db.Complains.AverageAsync(c => (decimal?)c.SentimentScore) ?? 0m;

            var query = _db.Complains.AsQueryable();

            if (!string.IsNullOrEmpty(department))
                query = query.Where(c => c.Department == department);

            if (!string.IsNullOrEmpty(priority))
                query = query.Where(c => c.Priority == priority);

            if (!string.IsNullOrEmpty(category))
                query = query.Where(c => c.Category == category);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(c => c.Status == status);

            var filteredCount = await query.CountAsync();

            query = sortBy switch
            {
                "date_asc"      => query.OrderBy(c => c.CreatedAt),
                "priority_desc" => query.OrderByDescending(c => c.Priority == "low" ? 1 : c.Priority == "medium" ? 2 : 3),
                "priority_asc"  => query.OrderBy(c => c.Priority == "low" ? 1 : c.Priority == "medium" ? 2 : 3),
                "category_asc"  => query.OrderBy(c => c.Category),
                "category_desc" => query.OrderByDescending(c => c.Category),
                "status_asc"    => query.OrderBy(c => c.Status),
                "status_desc"   => query.OrderByDescending(c => c.Status),
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
                Complains         = complains,
                TotalCount        = totalCount,
                HighPriorityCount = highPriorityCount,
                AvgSentiment      = avgSentiment,
                FilteredCount     = filteredCount,
                CurrentPage       = page,
                PageSize          = pageSize,
                TotalPages        = totalPages,
                SortBy            = sortBy,
                Department        = department,
                Priority          = priority,
                Category          = category,
                Status            = status
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
