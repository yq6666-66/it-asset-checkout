using ItAssetCheckout.Data;
using ItAssetCheckout.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItAssetCheckout.Controllers;

// Create, read, update, delete, and search staff records.
[Authorize]
public class StaffController : Controller
{
    private readonly ApplicationDbContext _db;

    // Stores the database used by the staff actions.
    public StaffController(ApplicationDbContext db)
    {
        _db = db;
    }

    // Lists staff, filtered by the search box and ordered by the selected column.
    [HttpGet]
    public async Task<IActionResult> Index(string? q, string? sort)
    {
        var queryText = (q ?? string.Empty).Trim();
        var sortKey = sort switch
        {
            "name" => "name",
            "department" => "department",
            _ => "code"
        };

        var staff = _db.Staff.AsQueryable();
        if (queryText.Length > 0)
        {
            var term = queryText.ToLower();
            staff = staff.Where(person =>
                person.StaffCode.ToLower().Contains(term)
                || person.FullName.ToLower().Contains(term)
                || person.Email.ToLower().Contains(term)
                || person.Department.ToLower().Contains(term));
        }

        staff = sortKey switch
        {
            "name" => staff.OrderBy(person => person.FullName).ThenBy(person => person.StaffCode),
            "department" => staff.OrderBy(person => person.Department).ThenBy(person => person.StaffCode),
            _ => staff.OrderBy(person => person.StaffCode)
        };

        return View(new StaffListViewModel
        {
            Staff = await staff.ToListAsync(),
            Query = queryText,
            Sort = sortKey
        });
    }

    // Shows one staff member.
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var person = await _db.Staff.FindAsync(id);
        if (person is null)
        {
            return NotFound();
        }

        return View(person);
    }

    // Shows the blank staff form.
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Staff());
    }

    // Validates and stores a new staff member.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("StaffCode,FullName,Email,Department")] Staff person)
    {
        if (!await PrepareAsync(person, excludingId: null))
        {
            return View(person);
        }

        _db.Staff.Add(person);
        await _db.SaveChangesAsync();
        TempData["Message"] = "Staff member saved.";
        return RedirectToAction(nameof(Index));
    }

    // Loads a staff member into the edit form.
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var person = await _db.Staff.FindAsync(id);
        if (person is null)
        {
            return NotFound();
        }

        return View(person);
    }

    // Validates and updates an existing staff member.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,StaffCode,FullName,Email,Department")] Staff person)
    {
        if (id != person.Id)
        {
            return NotFound();
        }

        if (!await _db.Staff.AnyAsync(existing => existing.Id == id))
        {
            return NotFound();
        }

        if (!await PrepareAsync(person, excludingId: id))
        {
            return View(person);
        }

        _db.Update(person);
        await _db.SaveChangesAsync();
        TempData["Message"] = "Staff member updated.";
        return RedirectToAction(nameof(Index));
    }

    // Asks for confirmation before deleting a staff member.
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var person = await _db.Staff.FindAsync(id);
        if (person is null)
        {
            return NotFound();
        }

        ViewData["CanDelete"] = !await _db.Checkouts.AnyAsync(checkout => checkout.StaffId == id);
        return View(person);
    }

    // Deletes the staff member after confirmation.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var person = await _db.Staff.FindAsync(id);
        if (person is null)
        {
            return NotFound();
        }

        if (await _db.Checkouts.AnyAsync(checkout => checkout.StaffId == id))
        {
            TempData["Message"] = "This staff member has checkout records, so they cannot be deleted.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        _db.Staff.Remove(person);
        await _db.SaveChangesAsync();
        TempData["Message"] = "Staff member deleted.";
        return RedirectToAction(nameof(Index));
    }

    // Trims input, re-runs validation, and rejects a duplicate staff code.
    private async Task<bool> PrepareAsync(Staff person, int? excludingId)
    {
        person.StaffCode = (person.StaffCode ?? string.Empty).Trim().ToUpperInvariant();
        person.FullName = (person.FullName ?? string.Empty).Trim();
        person.Email = (person.Email ?? string.Empty).Trim();
        person.Department = (person.Department ?? string.Empty).Trim();

        ModelState.Clear();
        if (!TryValidateModel(person))
        {
            return false;
        }

        var duplicates = _db.Staff.Where(existing => existing.StaffCode == person.StaffCode);
        if (excludingId.HasValue)
        {
            var id = excludingId.Value;
            duplicates = duplicates.Where(existing => existing.Id != id);
        }

        if (await duplicates.AnyAsync())
        {
            ModelState.AddModelError(nameof(Staff.StaffCode), "A staff member with this code already exists.");
            return false;
        }

        return true;
    }
}
