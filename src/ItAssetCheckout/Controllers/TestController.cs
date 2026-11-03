using ItAssetCheckout.Models;
using Microsoft.AspNetCore.Mvc;

namespace ItAssetCheckout.Controllers;

// Lists the xUnit cases. The tests themselves run with dotnet test.
public class TestController : Controller
{
    // Shows the unit-test catalog and the command that runs it.
    [HttpGet]
    public IActionResult Index()
    {
        return View(UnitTestCatalog.Cases);
    }
}
