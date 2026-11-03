using ItAssetCheckout.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ItAssetCheckout.Controllers;

// Cookie sign-in and sign-out for the demo environment.
public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;

    // Stores the Identity sign-in helper used by login and logout.
    public AccountController(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    // Shows the sign-in form. Already signed-in users go home.
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    // Checks the username and password, then issues the authentication cookie.
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            model.UserName,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "This account is temporarily locked. Try again later.");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "Invalid username or password.");
        return View(model);
    }

    // Clears the authentication cookie.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // Shown when a signed-in user is not allowed to open a page.
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
