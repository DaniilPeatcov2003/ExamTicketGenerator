using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

var baseUrl = args.FirstOrDefault() ?? "http://localhost:5080";
var options = new ChromeOptions();
options.AddArgument("--headless=new");
options.AddArgument("--window-size=1440,1000");
options.AddArgument("--no-sandbox");
options.AddArgument("--disable-dev-shm-usage");

using var driver = new ChromeDriver(options);
driver.Navigate().GoToUrl(baseUrl);

var lastName = driver.FindElement(By.Id("last-name"));
var firstName = driver.FindElement(By.Id("first-name"));
lastName.SendKeys("Selenium");
firstName.SendKeys("Test");
driver.FindElement(By.Id("submit-button")).Click();

var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(10));
wait.Until(_ => driver.FindElement(By.Id("ticket-number")).Text != "--");

var ticket = int.Parse(driver.FindElement(By.Id("ticket-number")).Text);
var status = driver.FindElement(By.Id("status")).Text;
var journal = driver.FindElement(By.Id("journal-body")).Text;

if (ticket is < 1 or > 20)
{
    throw new InvalidOperationException($"Ticket number is outside the expected range: {ticket}");
}

if (!status.Contains("сохранён", StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException($"Unexpected status: {status}");
}

if (!journal.Contains("Selenium Test", StringComparison.Ordinal))
{
    throw new InvalidOperationException("The student name was not rendered in the journal.");
}

Console.WriteLine($"Selenium smoke test passed: ticket {ticket}, status '{status}'");
