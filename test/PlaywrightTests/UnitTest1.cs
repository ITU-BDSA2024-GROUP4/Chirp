using System.Text.RegularExpressions;

using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;


namespace PlaywrightTests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public partial class ExampleTest : PageTest
    {

        private ChirpWebfactory _factory = null!;

        public override BrowserNewContextOptions ContextOptions()
        {
            return new BrowserNewContextOptions
            {
                IgnoreHTTPSErrors = true
            };
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _factory = new ChirpWebfactory();
            _factory.Start();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _factory.Dispose();
        }


        [Test]
        public async Task HasTitle()
        {

            _ = await Page.GotoAsync(_factory.GetBaseAddress());

            // Expect a title "to contain" a substring.
            await Expect(Page).ToHaveTitleAsync("Chirp!");
        }

        [Test]
        public async Task HasHeader()
        {
            await Page.GotoAsync(_factory.GetBaseAddress());

            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "login" })).ToBeVisibleAsync();

            // Expect a header to contain a substring.
        }
        
    }
}