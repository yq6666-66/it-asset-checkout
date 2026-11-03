using ItAssetCheckout.Data;
using ItAssetCheckout.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ItAssetCheckout.Controllers;

// Create, read, update, and delete equipment loans.
[Authorize]
public class CheckoutsController : Controller
{
    private readonly ApplicationDbContext _db;

    // Stores the database used by the checkout actions.
    public CheckoutsController(ApplicationDbContext db)
    {
        _db = db;
    }

    // Lists checkouts, filtered by the search box and ordered by the selected column.
    [HttpGet]
    public async Task<IActionResult> Index(string? q, string? sort)
    {
        var queryText = (q ?? string.Empty).Trim();
        var sortKey = sort switch
        {
            "asset" => "asset",
            "staff" => "staff",
            "due" => "due",
            "return" => "return",
            _ => "checkout"
        };

        var checkouts = _db.Checkouts
            .Include(checkout => checkout.Asset)
            .Include(checkout => checkout.Staff)
            .AsQueryable();

        if (queryText.Length > 0)
        {
            var term = queryText.ToLower();
            var wantsOpen = term == "open";
            var day = DateTime.TryParseExact(
                queryText,
                "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var parsed)
                ? parsed.Date
                : (DateTime?)null;

            checkouts = checkouts.Where(checkout =>
                (checkout.Asset != null && (
                    checkout.Asset.TagNo.ToLower().Contains(term)
                    || checkout.Asset.Name.ToLower().Contains(term)))
                || (checkout.Staff != null && (
                    checkout.Staff.StaffCode.ToLower().Contains(term)
                    || checkout.Staff.FullName.ToLower().Contains(term)))
                || (checkout.Note != null && checkout.Note.ToLower().Contains(term))
                || (wantsOpen && checkout.ReturnAt == null)
                || (day != null && (
                    checkout.CheckoutAt.Date == day
                    || checkout.DueAt.Date == day
                    || (checkout.ReturnAt != null && checkout.ReturnAt.Value.Date == day))));
        }

        checkouts = sortKey switch
        {
            "asset" => checkouts.OrderBy(checkout => checkout.Asset!.TagNo).ThenByDescending(checkout => checkout.CheckoutAt),
            "staff" => checkouts.OrderBy(checkout => checkout.Staff!.FullName).ThenBy(checkout => checkout.Staff!.StaffCode),
            "due" => checkouts.OrderBy(checkout => checkout.DueAt).ThenBy(checkout => checkout.Id),
            "return" => checkouts.OrderBy(checkout => checkout.ReturnAt).ThenBy(checkout => checkout.Id),
            _ => checkouts.OrderByDescending(checkout => checkout.CheckoutAt).ThenByDescending(checkout => checkout.Id)
        };

        return View(new CheckoutListViewModel
        {
            Checkouts = await checkouts.ToListAsync(),
            Query = queryText,
            Sort = sortKey
        });
    }

    // Shows one checkout.
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var checkout = await FindAsync(id);
        if (checkout is null)
        {
            return NotFound();
        }

        return View(checkout);
    }

    // Shows the checkout form with today's date and a two-week due date.
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadChoicesAsync();
        return View(new Checkout
        {
            CheckoutAt = DateTime.Today,
            DueAt = DateTime.Today.AddDays(14)
        });
    }

    // Validates and stores a new checkout, including the one-open-checkout rule.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("AssetId,StaffId,CheckoutAt,DueAt,ReturnAt,Note")] Checkout checkout)
    {
        if (!await PrepareAsync(checkout, excludingId: null))
        {
            await LoadChoicesAsync();
            return View(checkout);
        }

        _db.Checkouts.Add(checkout);
        if (!await SaveAsync(checkout))
        {
            await LoadChoicesAsync();
            return View(checkout);
        }

        TempData["Message"] = "Checkout saved.";
        return RedirectToAction(nameof(Index));
    }

    // Loads a checkout into the edit form.
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var checkout = await _db.Checkouts.FindAsync(id);
        if (checkout is null)
        {
            return NotFound();
        }

        await LoadChoicesAsync();
        return View(checkout);
    }

    // Validates and updates a checkout, including the one-open-checkout rule.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,AssetId,StaffId,CheckoutAt,DueAt,ReturnAt,Note")] Checkout checkout)
    {
        if (id != checkout.Id || !await _db.Checkouts.AnyAsync(existing => existing.Id == id))
        {
            return NotFound();
        }

        if (!await PrepareAsync(checkout, excludingId: id))
        {
            await LoadChoicesAsync();
            return View(checkout);
        }

        _db.Update(checkout);
        if (!await SaveAsync(checkout))
        {
            await LoadChoicesAsync();
            return View(checkout);
        }

        TempData["Message"] = "Checkout updated.";
        return RedirectToAction(nameof(Index));
    }

    // Asks for confirmation before deleting a checkout.
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var checkout = await FindAsync(id);
        if (checkout is null)
        {
            return NotFound();
        }

        return View(checkout);
    }

    // Deletes the checkout after confirmation.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var checkout = await _db.Checkouts.FindAsync(id);
        if (checkout is null)
        {
            return NotFound();
        }

        _db.Checkouts.Remove(checkout);
        await _db.SaveChangesAsync();
        TempData["Message"] = "Checkout deleted.";
        return RedirectToAction(nameof(Index));
    }

    // Loads a checkout with its asset and staff member.
    private Task<Checkout?> FindAsync(int id)
    {
        return _db.Checkouts
            .Include(checkout => checkout.Asset)
            .Include(checkout => checkout.Staff)
            .FirstOrDefaultAsync(checkout => checkout.Id == id);
    }

    // Fills the asset and staff dropdowns.
    private async Task LoadChoicesAsync()
    {
        ViewBag.AssetChoices = await _db.Assets
            .OrderBy(asset => asset.TagNo)
            .Select(asset => new SelectListItem($"{asset.TagNo} — {asset.Name}", asset.Id.ToString()))
            .ToListAsync();
        ViewBag.StaffChoices = await _db.Staff
            .OrderBy(staff => staff.StaffCode)
            .Select(staff => new SelectListItem($"{staff.StaffCode} — {staff.FullName}", staff.Id.ToString()))
            .ToListAsync();
    }

    // Trims the note, checks dates and foreign keys, and rejects a second open checkout.
    private async Task<bool> PrepareAsync(Checkout checkout, int? excludingId)
    {
        checkout.Note = string.IsNullOrWhiteSpace(checkout.Note) ? null : checkout.Note.Trim();
        if (checkout.CheckoutAt != default)
        {
            checkout.CheckoutAt = checkout.CheckoutAt.Date;
        }

        if (checkout.DueAt != default)
        {
            checkout.DueAt = checkout.DueAt.Date;
        }

        if (checkout.ReturnAt is DateTime returned)
        {
            checkout.ReturnAt = returned.Date;
        }

        ModelState.Clear();
        if (!TryValidateModel(checkout))
        {
            return false;
        }

        if (checkout.AssetId <= 0 || !await _db.Assets.AnyAsync(asset => asset.Id == checkout.AssetId))
        {
            ModelState.AddModelError(nameof(Checkout.AssetId), "Select an asset.");
            return false;
        }

        if (checkout.StaffId <= 0 || !await _db.Staff.AnyAsync(staff => staff.Id == checkout.StaffId))
        {
            ModelState.AddModelError(nameof(Checkout.StaffId), "Select a staff member.");
            return false;
        }

        var anotherOpen = await HasAnotherOpenCheckoutAsync(checkout.AssetId, excludingId);
        if (CheckoutRules.IsSecondOpenCheckout(checkout.ReturnAt, anotherOpen))
        {
            ModelState.AddModelError(nameof(Checkout.AssetId), "This asset already has an open checkout.");
            return false;
        }

        return true;
    }

    // True when a different checkout for the asset is still open.
    private async Task<bool> HasAnotherOpenCheckoutAsync(int assetId, int? excludingId)
    {
        var open = _db.Checkouts.Where(checkout => checkout.AssetId == assetId && checkout.ReturnAt == null);
        if (excludingId.HasValue)
        {
            var id = excludingId.Value;
            open = open.Where(checkout => checkout.Id != id);
        }

        return await open.AnyAsync();
    }

    // Saves the checkout. A unique-index collision is reported as the open-checkout rule.
    private async Task<bool> SaveAsync(Checkout checkout)
    {
        try
        {
            await _db.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(Checkout.AssetId), "This asset already has an open checkout.");
            return false;
        }
    }
}
