using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimplestTwilio.Data;
using SimplestTwilio.Models;
using SimplestTwilio.Services;

namespace SimplestTwilio.Controllers;

public class MessagesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ITwilioService _twilioService;
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(
        ApplicationDbContext context,
        ITwilioService twilioService,
        ILogger<MessagesController> logger)
    {
        _context = context;
        _twilioService = twilioService;
        _logger = logger;
    }

    // GET: Messages
    public async Task<IActionResult> Index()
    {
        try
        {
            var messages = await _context.Messages
                .AsNoTracking()
                .Include(m => m.Histories)
                .OrderByDescending(m => m.CreatedDate)
                .ToListAsync();

            var viewModel = new MessageIndexViewModel
            {
                TwilioConfigured = _twilioService.ValidateConfiguration(),
                Messages = messages.Select(m => new MessageSummary
                {
                    MessageId = m.MessageId,
                    Text = m.Text,
                    TextPreview = m.Text.Length > 100 ? m.Text.Substring(0, 100) + "..." : m.Text,
                    CreatedDate = m.CreatedDate,
                    TimesSent = m.Histories.Count
                }).ToList()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading messages");
            TempData["ErrorMessage"] = "An error occurred while loading messages.";
            return View(new MessageIndexViewModel());
        }
    }

        // GET: Messages/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

    // GET: Messages/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Messages/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Message model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            model.CreatedDate = DateTime.UtcNow;
            _context.Messages.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Message template created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating message");
            ModelState.AddModelError("", "An error occurred while creating the message.");
            return View(model);
        }
    }

    // GET: Messages/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var message = await _context.Messages
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MessageId == id);

            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading message for edit");
            TempData["ErrorMessage"] = "An error occurred while loading the message.";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Messages/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Message model)
    {
        if (id != model.MessageId)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            message.Text = model.Text;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Message template updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating message");
            ModelState.AddModelError("", "An error occurred while updating the message.");
            return View(model);
        }
    }

    // GET: Messages/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var message = await _context.Messages
                .Include(m => m.Histories)
                .FirstOrDefaultAsync(m => m.MessageId == id);

            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading message for delete");
            TempData["ErrorMessage"] = "An error occurred while loading the message.";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Messages/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Message template deleted successfully. History records have been preserved.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting message");
            TempData["ErrorMessage"] = "An error occurred while deleting the message.";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Messages/Send/5
    public async Task<IActionResult> Send(int id)
    {
        try
        {
            var message = await _context.Messages
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MessageId == id);

            if (message == null)
            {
                return NotFound();
            }

            if (!_twilioService.ValidateConfiguration())
            {
                TempData["ErrorMessage"] = "Twilio is not configured. Please set up your credentials before sending messages.";
                return RedirectToAction(nameof(Index));
            }

            var lists = await _context.RecipientLists
                .AsNoTracking()
                .Include(l => l.Contacts)
                .OrderBy(l => l.Name)
                .ToListAsync();

            var viewModel = new SendMessageViewModel
            {
                MessageId = message.MessageId,
                MessageText = message.Text,
                AvailableLists = lists.Select(l => new RecipientListOption
                {
                    RecipientListId = l.RecipientListId,
                    Name = l.Name,
                    ContactCount = l.Contacts.Count
                }).ToList()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading send page");
            TempData["ErrorMessage"] = "An error occurred while loading the send page.";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Messages/SendConfirm
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendConfirm(SendMessageViewModel model)
    {
        if (model.SelectedListIds == null || !model.SelectedListIds.Any())
        {
            ModelState.AddModelError("SelectedListIds", "Please select at least one recipient list");
        }

        if (!ModelState.IsValid)
        {
            // Reload available lists
            var lists = await _context.RecipientLists
                .AsNoTracking()
                .Include(l => l.Contacts)
                .OrderBy(l => l.Name)
                .ToListAsync();

            model.AvailableLists = lists.Select(l => new RecipientListOption
            {
                RecipientListId = l.RecipientListId,
                Name = l.Name,
                ContactCount = l.Contacts.Count,
                IsSelected = model.SelectedListIds?.Contains(l.RecipientListId) ?? false
            }).ToList();

            return View("Send", model);
        }

        try
        {
            var message = await _context.Messages
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MessageId == model.MessageId);

            if (message == null)
            {
                return NotFound();
            }

            var selectedLists = await _context.RecipientLists
                .AsNoTracking()
                .Include(l => l.Contacts)
                .Where(l => model.SelectedListIds.Contains(l.RecipientListId))
                .ToListAsync();

            // Calculate totals
            model.MessageText = message.Text;
            model.TotalRecipients = selectedLists.Sum(l => l.Contacts.Count);
            model.SmsSegments = _twilioService.CalculateSmsSegments(message.Text);
            model.TotalSmsCount = model.TotalRecipients * model.SmsSegments;
            model.AvailableLists = selectedLists.Select(l => new RecipientListOption
            {
                RecipientListId = l.RecipientListId,
                Name = l.Name,
                ContactCount = l.Contacts.Count,
                IsSelected = true
            }).ToList();

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing send confirmation");
            TempData["ErrorMessage"] = "An error occurred while preparing the send confirmation.";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Messages/SendExecute
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendExecute(SendMessageViewModel model)
    {
        try
        {
            if (!_twilioService.ValidateConfiguration())
            {
                TempData["ErrorMessage"] = "Twilio is not configured properly.";
                return RedirectToAction(nameof(Index));
            }

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.MessageId == model.MessageId);

            if (message == null)
            {
                return NotFound();
            }

            var selectedLists = await _context.RecipientLists
                .Include(l => l.Contacts)
                .Where(l => model.SelectedListIds.Contains(l.RecipientListId))
                .ToListAsync();

            var resultViewModel = new SendResultViewModel
            {
                MessageText = message.Text,
                ListNames = selectedLists.Select(l => l.Name).ToList()
            };

            // Send to each list and track results
            foreach (var list in selectedLists)
            {
                var recipients = list.Contacts.Select(c => c.PhoneNumber).ToList();
                
                var sendResult = await _twilioService.SendBulkSmsAsync(recipients, message.Text);

                // Update failures with contact names
                foreach (var failure in sendResult.Failures)
                {
                    var contact = list.Contacts.FirstOrDefault(c => c.PhoneNumber == failure.PhoneNumber);
                    if (contact != null)
                    {
                        failure.ContactName = contact.Name;
                    }
                }

                // Create history record
                var history = new History
                {
                    MessageId = message.MessageId,
                    RecipientListId = list.RecipientListId,
                    SentDate = DateTime.UtcNow,
                    TotalRecipients = sendResult.TotalRecipients,
                    SuccessfulSends = sendResult.SuccessfulSends,
                    FailedSends = sendResult.FailedSends,
                    Status = sendResult.FailedSends == 0 ? "Completed" : "Completed with Errors",
                    ErrorMessage = sendResult.FailedSends > 0 ? $"{sendResult.FailedSends} messages failed to send" : null
                };

                _context.Histories.Add(history);

                // Aggregate results
                resultViewModel.TotalRecipients += sendResult.TotalRecipients;
                resultViewModel.SuccessfulSends += sendResult.SuccessfulSends;
                resultViewModel.FailedSends += sendResult.FailedSends;
                resultViewModel.Failures.AddRange(sendResult.Failures);
            }

            await _context.SaveChangesAsync();

            return View("SendResult", resultViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing bulk send");
            TempData["ErrorMessage"] = "An error occurred while sending messages.";
            return RedirectToAction(nameof(Index));
        }
    }
}

