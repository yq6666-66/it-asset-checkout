using ItAssetCheckout.Data;
using ItAssetCheckout.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItAssetCheckout.Controllers;

// Create, read, update, and delete IT asset records.
[Authorize]
public class AssetsController : Controller
{
    private readonly ApplicationDbContext _db;

    // Stores the database used by the asset actions.
    public AssetsController(ApplicationDbContext db)
    {
        _db = db;
    }

    // Lists assets, filtered by the search box and ordered by the selected column.
    [HttpGet]
    public async Task<IActionResult> Index(string? q, string? sort)
    {
        var queryText = (q ?? string.Empty).Trim();
        var sortKey = sort switch
        {
            "name" => "name",
            "category" => "category",
            "status" => "status",
            _ => "tag"
        };

        var assets = _db.Assets.AsQueryable();
        if (queryText.Length > 0)
        {
            var term = queryText.ToLower();
            assets = assets.Where(asset =>
                asset.TagNo.ToLower().Contains(term)
                || asset.Name.ToLower().Contains(term)
                || asset.Category.ToLower().Contains(term)
                || asset.Status.ToLower().Contains(term));
        }

        assets = sortKey switch
        {
            "name" => assets.OrderBy(asset => asset.Name).ThenBy(asset => asset.TagNo),
            "category" => assets.OrderBy(asset => asset.Category).ThenBy(asset => asset.TagNo),
            "status" => assets.OrderBy(asset => asset.Status).ThenBy(asset => asset.TagNo),
            _ => assets.OrderBy(asset => asset.TagNo)
        };

        return View(new AssetListViewModel
        {
            Assets = await assets.ToListAsync(),
            Query = queryText,
            Sort = sortKey
        });
    }

    // Shows one asset.
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var asset = await _db.Assets.FindAsync(id);
        if (asset is null)
        {
            return NotFound();
        }

        return View(asset);
    }

    // Shows the blank asset form.
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Asset { PurchaseDate = DateTime.Today });
    }

    // Validates and stores a new asset.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TagNo,Name,Category,Status,PurchaseDate,Value")] Asset asset)
    {
        if (!await PrepareAsync(asset, excludingId: null))
        {
            return View(asset);
        }

        _db.Assets.Add(asset);
        await _db.SaveChangesAsync();
        TempData["Message"] = "Asset saved.";
        return RedirectToAction(nameof(Index));
    }

    // Loads an asset into the edit form.
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var asset = await _db.Assets.FindAsync(id);
        if (asset is null)
        {
            return NotFound();
        }

        return View(asset);
    }

    // Validates and updates an existing asset.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,TagNo,Name,Category,Status,PurchaseDate,Value")] Asset asset)
    {
        if (id != asset.Id)
        {
            return NotFound();
        }

        if (!await _db.Assets.AnyAsync(existing => existing.Id == id))
        {
            return NotFound();
        }

        if (!await PrepareAsync(asset, excludingId: id))
        {
            return View(asset);
        }

        _db.Update(asset);
        await _db.SaveChangesAsync();
        TempData["Message"] = "Asset updated.";
        return RedirectToAction(nameof(Index));
    }

    // Asks for confirmation before deleting an asset.
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var asset = await _db.Assets.FindAsync(id);
        if (asset is null)
        {
            return NotFound();
        }

        ViewData["CanDelete"] = !await _db.Checkouts.AnyAsync(checkout => checkout.AssetId == id);
        return View(asset);
    }

    // Deletes the asset after confirmation.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var asset = await _db.Assets.FindAsync(id);
        if (asset is null)
        {
            return NotFound();
        }

        if (await _db.Checkouts.AnyAsync(checkout => checkout.AssetId == id))
        {
            TempData["Message"] = "This asset has checkout records, so it cannot be deleted.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        _db.Assets.Remove(asset);
        await _db.SaveChangesAsync();
        TempData["Message"] = "Asset deleted.";
        return RedirectToAction(nameof(Index));
    }

    // Trims input, re-runs validation, and rejects a duplicate tag number.
    private async Task<bool> PrepareAsync(Asset asset, int? excludingId)
    {
        asset.TagNo = (asset.TagNo ?? string.Empty).Trim().ToUpperInvariant();
        asset.Name = (asset.Name ?? string.Empty).Trim();
        asset.Category = (asset.Category ?? string.Empty).Trim();
        asset.Status = (asset.Status ?? string.Empty).Trim();

        ModelState.Clear();
        if (!TryValidateModel(asset))
        {
            return false;
        }

        var duplicates = _db.Assets.Where(existing => existing.TagNo == asset.TagNo);
        if (excludingId.HasValue)
        {
            var id = excludingId.Value;
            duplicates = duplicates.Where(existing => existing.Id != id);
        }

        if (await duplicates.AnyAsync())
        {
            ModelState.AddModelError(nameof(Asset.TagNo), "An asset with this tag number already exists.");
            return false;
        }

        return true;
    }
}
