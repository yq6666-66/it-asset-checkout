using System.Diagnostics;
using ItAssetCheckout.Models;
using Microsoft.AspNetCore.Mvc;

namespace ItAssetCheckout.Controllers;

// Landing page and the production error view.
public class HomeController : Controller
{
    // Shows the branded home page.
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    // Shows a generic error page outside Development.
    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
