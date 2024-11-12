using System.Text.RegularExpressions;
using Microsoft.Playwright.NUnit;
using Microsoft.AspNetCore.Mvc.Testing;


namespace PlaywrightTests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public partial class ExampleTest : PageTest
    {

        private WebApplicationFactory<Program>? _factory;
        private string? _baseAddress;

        private HttpClient? _client;

        [SetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>();
            _factory.Server.Host.Start();

            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true, HandleCookies = true });
            _baseAddress = _client.BaseAddress.ToString();
        }

        [TearDown]
        public void TearDown()
        {
            _factory.Dispose();
            _client.Dispose();
        }


        [Test]
        public async Task HasTitle()
        {
            _ = await Page.GotoAsync($"{_baseAddress}");

            // Expect a title "to contain" a substring.
            await Expect(Page).ToHaveTitleAsync(MyRegex());
        }

        [GeneratedRegex("Chirp!")]
        private static partial Regex MyRegex();
    }
}