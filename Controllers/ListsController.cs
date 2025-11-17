using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimplestTwilio.Data;
using SimplestTwilio.Models;

namespace SimplestTwilio.Controllers;

public class ListsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ListsController> _logger;

    public ListsController(ApplicationDbContext context, ILogger<ListsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Lists
    public async Task<IActionResult> Index()
    {
        try
        {
            var lists = await _context.RecipientLists
                .Include(r => r.Contacts)
                .OrderByDescending(r => r.CreatedDate)
                .Select(r => new RecipientListSummary
                {
                    RecipientListId = r.RecipientListId,
                    Name = r.Name,
                    ContactCount = r.Contacts.Count,
                    CreatedDate = r.CreatedDate
                })
                .ToListAsync();

            var viewModel = new RecipientListsViewModel
            {
                Lists = lists
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recipient lists");
            return View(new RecipientListsViewModel());
        }
    }

    // GET: Lists/Details/5
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var list = await _context.RecipientLists
                .Include(r => r.Contacts)
                .FirstOrDefaultAsync(r => r.RecipientListId == id);

            if (list == null)
            {
                return NotFound();
            }

            var viewModel = new RecipientListDetailsViewModel
            {
                RecipientListId = list.RecipientListId,
                Name = list.Name,
                Contacts = list.Contacts.Select(c => new ContactSummary
                {
                    ContactId = c.ContactId,
                    PhoneNumber = c.PhoneNumber,
                    Name = c.Name
                }).ToList()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recipient list details for ID {ListId}", id);
            return NotFound();
        }
    }

    // GET: Lists/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Lists/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RecipientList model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            model.CreatedDate = DateTime.Now;
            _context.RecipientLists.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"List '{model.Name}' created successfully!";
            return RedirectToAction(nameof(Details), new { id = model.RecipientListId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating recipient list");
            ModelState.AddModelError("", "An error occurred while creating the list. Please try again.");
            return View(model);
        }
    }

    // GET: Lists/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var list = await _context.RecipientLists.FindAsync(id);

            if (list == null)
            {
                return NotFound();
            }

            return View(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recipient list for editing, ID {ListId}", id);
            return NotFound();
        }
    }

    // POST: Lists/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RecipientList model)
    {
        if (id != model.RecipientListId)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var existingList = await _context.RecipientLists.FindAsync(id);
            if (existingList == null)
            {
                return NotFound();
            }

            existingList.Name = model.Name;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"List '{model.Name}' updated successfully!";
            return RedirectToAction(nameof(Details), new { id = model.RecipientListId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating recipient list ID {ListId}", id);
            ModelState.AddModelError("", "An error occurred while updating the list. Please try again.");
            return View(model);
        }
    }

    // GET: Lists/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var list = await _context.RecipientLists
                .Include(r => r.Contacts)
                .FirstOrDefaultAsync(r => r.RecipientListId == id);

            if (list == null)
            {
                return NotFound();
            }

            return View(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recipient list for deletion, ID {ListId}", id);
            return NotFound();
        }
    }

    // POST: Lists/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var list = await _context.RecipientLists.FindAsync(id);
            if (list == null)
            {
                return NotFound();
            }

            var listName = list.Name;
            _context.RecipientLists.Remove(list);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"List '{listName}' and all its contacts have been deleted.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting recipient list ID {ListId}", id);
            TempData["ErrorMessage"] = "An error occurred while deleting the list. Please try again.";
            return RedirectToAction(nameof(Delete), new { id });
        }
    }

    // GET: Lists/AddContact
    public async Task<IActionResult> AddContact(int listId)
    {
        try
        {
            var list = await _context.RecipientLists.FindAsync(listId);
            if (list == null)
            {
                return NotFound();
            }

            ViewBag.ListId = listId;
            ViewBag.ListName = list.Name;
            return View(new Contact { RecipientListId = listId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading add contact form for list ID {ListId}", listId);
            return NotFound();
        }
    }

    // POST: Lists/AddContact
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddContact(Contact model)
    {
        _logger.LogInformation("AddContact POST called with RecipientListId: {ListId}, Phone: {Phone}, Name: {Name}", 
            model.RecipientListId, model.PhoneNumber, model.Name);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is invalid. Errors: {Errors}", 
                string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            
            var list = await _context.RecipientLists.FindAsync(model.RecipientListId);
            if (list != null)
            {
                ViewBag.ListId = model.RecipientListId;
                ViewBag.ListName = list.Name;
            }
            return View(model);
        }

        try
        {
            model.CreatedDate = DateTime.Now;
            _logger.LogInformation("Adding contact to database...");
            _context.Contacts.Add(model);
            
            var result = await _context.SaveChangesAsync();
            _logger.LogInformation("SaveChanges returned: {Result}", result);

            TempData["SuccessMessage"] = "Contact added successfully!";
            return RedirectToAction(nameof(Details), new { id = model.RecipientListId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding contact to list ID {ListId}. Exception: {Message}", 
                model.RecipientListId, ex.Message);
            ModelState.AddModelError("", $"An error occurred while adding the contact: {ex.Message}");
            
            var list = await _context.RecipientLists.FindAsync(model.RecipientListId);
            if (list != null)
            {
                ViewBag.ListId = model.RecipientListId;
                ViewBag.ListName = list.Name;
            }
            return View(model);
        }
    }

    // POST: Lists/RemoveContact
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveContact(int id)
    {
        try
        {
            var contact = await _context.Contacts
                .Include(c => c.RecipientList)
                .FirstOrDefaultAsync(c => c.ContactId == id);

            if (contact == null)
            {
                return NotFound();
            }

            var listId = contact.RecipientListId;
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Contact removed successfully!";
            return RedirectToAction(nameof(Details), new { id = listId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing contact ID {ContactId}", id);
            TempData["ErrorMessage"] = "An error occurred while removing the contact. Please try again.";
            return RedirectToAction(nameof(Index));
        }
    }
}
