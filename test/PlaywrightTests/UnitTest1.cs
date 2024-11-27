using System.Runtime.CompilerServices;

using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class ExampleTest : PageTest
    {
        private ChirpWebfactory _factory = null!;

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
        public async Task Can_Login_And_Logout_As_Helge()
        {
            // Arrange
            await Page.GotoAsync(_factory.GetBaseAddress());

            // Act
            await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("ropf@itu.dk");
            await Page.GetByPlaceholder("password").FillAsync("LetM31n!");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

            // Assert
            await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "What's on your mind Helge?" })).ToBeVisibleAsync();
            
            await Page.GetByRole(AriaRole.Link, new (){Name = "logout [Helge]"}).ClickAsync();
            
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "login" })).ToBeVisibleAsync();
        }

        [Test]
        public async Task Can_Login_And_Logout_As_Adrian()
        {
            // Arrange
            await Page.GotoAsync(_factory.GetBaseAddress());

            // Act
            await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("adho@itu.dk");
            await Page.GetByPlaceholder("password").FillAsync("M32Want_Access");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

            // Assert
            await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "What's on your mind Adrian?" })).ToBeVisibleAsync();
            
            await Page.GetByRole(AriaRole.Link, new (){Name = "logout [Adrian]"}).ClickAsync();
            
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "login" })).ToBeVisibleAsync();
        }
        
       
    }
}