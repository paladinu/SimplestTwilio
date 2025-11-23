using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimplestTwilio.Data;
using SimplestTwilio.Models;

namespace SimplestTwilio.Controllers;

public class HistoryController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HistoryController> _logger;

    public HistoryController(ApplicationDbContext context, ILogger<HistoryController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: History
    public async Task<IActionResult> Index()
    {
        try
        {
            var histories = await _context.Histories
                .AsNoTracking()
                .Include(h => h.Message)
                .Include(h => h.RecipientList)
                .OrderByDescending(h => h.SentDate)
                .ToListAsync();

            var viewModel = new HistoryIndexViewModel
            {
                Histories = histories.Select(h => new HistorySummary
                {
                    HistoryId = h.HistoryId,
                    MessageText = h.Message?.Text ?? "[Deleted Message]",
                    ListName = h.RecipientList?.Name ?? "[Deleted List]",
                    SentDate = h.SentDate,
                    TotalRecipients = h.TotalRecipients,
                    SuccessfulSends = h.SuccessfulSends,
                    FailedSends = h.FailedSends,
                    Status = h.Status
                }).ToList()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading history");
            TempData["ErrorMessage"] = "An error occurred while loading history.";
            return View(new HistoryIndexViewModel());
        }
    }

    // GET: History/Details/5
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var history = await _context.Histories
                .Include(h => h.Message)
                .Include(h => h.RecipientList)
                .FirstOrDefaultAsync(h => h.HistoryId == id);

            if (history == null)
            {
                return NotFound();
            }

            return View(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading history details");
            TempData["ErrorMessage"] = "An error occurred while loading history details.";
            return RedirectToAction(nameof(Index));
        }
    }

}