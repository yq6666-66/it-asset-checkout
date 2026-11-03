using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ItAssetCheckout.Selenium;

// Skips the browser cases unless RUN_SELENIUM=1, so a normal dotnet test stays on the unit project.
public sealed class SeleniumFactAttribute : FactAttribute
{
    public SeleniumFactAttribute()
    {
        if (Environment.GetEnvironmentVariable("RUN_SELENIUM") != "1")
        {
            Skip = "Set RUN_SELENIUM=1 while the app is listening on http://127.0.0.1:5080.";
        }
    }
}

// Browser checks for the local demo app. They run only when RUN_SELENIUM=1.
public class UiTests : IDisposable
{
    private const string BaseUrl = "http://127.0.0.1:5080";
    private readonly IWebDriver? _driver;
    private readonly string _shotDir = string.Empty;
    private readonly string _userName = "admin";
    private readonly string _password = string.Empty;

    public UiTests()
    {
        if (Environment.GetEnvironmentVariable("RUN_SELENIUM") != "1")
        {
            return;
        }

        var configuredUser = Environment.GetEnvironmentVariable("DEMO_ADMIN_USER");
        if (!string.IsNullOrWhiteSpace(configuredUser))
        {
            _userName = configuredUser;
        }

        var configuredPassword = Environment.GetEnvironmentVariable("DEMO_ADMIN_PASSWORD");
        if (string.IsNullOrWhiteSpace(configuredPassword))
        {
            throw new InvalidOperationException(
                "DEMO_ADMIN_PASSWORD is required when RUN_SELENIUM=1. See .env.example. Demo environment only.");
        }

        _password = configuredPassword;
        _shotDir = Environment.GetEnvironmentVariable("SELENIUM_SHOT_DIR")
            ?? Path.Combine(Directory.GetCurrentDirectory(), "screenshots");
        Directory.CreateDirectory(_shotDir);
        Directory.CreateDirectory(Path.Combine(_shotDir, "failures"));

        var options = new ChromeOptions();
        if (File.Exists("/usr/local/bin/google-chrome"))
        {
            options.BinaryLocation = "/usr/local/bin/google-chrome";
        }

        options.AddArgument("--headless=new");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--window-size=1280,900");
        _driver = new ChromeDriver(options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
    }

    [SeleniumFact(DisplayName = "SEL-01 Login shows the demo username")]
    public void SEL01_Login_ShowsAdmin()
    {
        Login();
        Check("sel-01-login", () =>
        {
            var account = Driver.FindElement(By.CssSelector(".account")).Text;
            Assert.Contains(_userName, account);
            Assert.Contains("Logout", account);
        });
    }

    [SeleniumFact(DisplayName = "SEL-02 Assets list shows a seeded laptop")]
    public void SEL02_Assets_ShowsSeedRow()
    {
        Login();
        Driver.Navigate().GoToUrl(BaseUrl + "/Assets");
        Check("sel-02-assets", () =>
        {
            var body = Driver.FindElement(By.TagName("body")).Text;
            Assert.Contains("IT-10482", body);
            Assert.Contains("Dell Latitude 5440", body);
        });
    }

    [SeleniumFact(DisplayName = "SEL-03 Checkouts list shows the open seeded loan")]
    public void SEL03_Checkouts_ShowsOpenSeedRow()
    {
        Login();
        Driver.Navigate().GoToUrl(BaseUrl + "/Checkouts");
        Check("sel-03-checkouts", () =>
        {
            var body = Driver.FindElement(By.TagName("body")).Text;
            Assert.Contains("IT-10490", body);
            Assert.Contains("Open", body);
            Assert.Contains("Ben Cho", body);
        });
    }

    [SeleniumFact(DisplayName = "SEL-04 Staff list shows a seeded person")]
    public void SEL04_Staff_ShowsSeedRow()
    {
        Login();
        Driver.Navigate().GoToUrl(BaseUrl + "/Staff");
        Check("sel-04-staff", () =>
        {
            var body = Driver.FindElement(By.TagName("body")).Text;
            Assert.Contains("S-1001", body);
            Assert.Contains("Ada Okonkwo", body);
        });
    }

    public void Dispose()
    {
        _driver?.Quit();
    }

    private IWebDriver Driver => _driver ?? throw new InvalidOperationException("Browser was not started.");

    private void Login()
    {
        Driver.Navigate().GoToUrl(BaseUrl + "/Account/Login");
        Driver.FindElement(By.Id("UserName")).SendKeys(_userName);
        Driver.FindElement(By.Id("Password")).SendKeys(_password);
        Driver.FindElement(By.CssSelector("form.form-card button[type=submit]")).Click();
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (DateTime.UtcNow < deadline)
        {
            if (Driver.FindElements(By.CssSelector(".username")).Count > 0)
            {
                return;
            }

            Thread.Sleep(100);
        }

        throw new InvalidOperationException("Sign-in did not show the username.");
    }

    private void Check(string name, Action assert)
    {
        try
        {
            assert();
            Save(name);
        }
        catch (Exception ex)
        {
            var path = Save(Path.Combine("failures", name));
            throw new Exception("Screenshot: " + path + ". " + ex.Message, ex);
        }
    }

    private string Save(string name)
    {
        var path = Path.Combine(_shotDir, name + ".png");
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var shot = ((ITakesScreenshot)Driver).GetScreenshot();
        shot.SaveAsFile(path);
        return path;
    }
}
