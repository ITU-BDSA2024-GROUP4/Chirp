using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;


namespace PlaywrightTests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class UITests : PageTest
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

        [Test]
        public async Task Gets_Redirected_To_Github()
        {
            // Arrange
            await Page.GotoAsync(_factory.GetBaseAddress());
            
            // Act
            await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
            await Page.GetByRole(AriaRole.Button, new() { Name = "GitHub" }).ClickAsync();

            // Assert
            await Expect(Page).ToHaveURLAsync(new Regex(@".*github.com/.*"));
        }

        [Test]
        public async Task Can_Post_As_User()
        {
            // Arrange
            var message = Faker.Lorem.Sentence();
            var user = "Helge";
            await Page.GotoAsync(_factory.GetBaseAddress());

            // Act
            await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("ropf@itu.dk");
            await Page.GetByPlaceholder("password").FillAsync("LetM31n!");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
            await Page.Locator("#MessageInput").ClickAsync();
            await Page.Locator("#MessageInput").FillAsync($"{message}");
            await Page.Locator("#MessageInput").PressAsync("Enter");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

            
            // Assert
            var postLocator = Page.Locator("li").Filter(new() { HasText=$"{user}" }).First;
            await Expect(postLocator).ToHaveTextAsync(new Regex($".*{message}.*"));            
            
        }
        [Test]
        public async Task End_To_End_Test()
        {
            // Arrange
            var username = Faker.Name.First();
            var email = Faker.Internet.Email();
            var password = $"{Faker.Name.First()}!{Faker.Name.Last()}{Faker.RandomNumber.Next()}";
            var message = Faker.Lorem.Sentence();
            await Page.GotoAsync(_factory.GetBaseAddress());

            // Act
            // Register
            await Page.GetByRole(AriaRole.Link, new() { Name = "register", Exact = true }).ClickAsync();
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync(email);
            await Page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
            await Page.GetByPlaceholder("name", new() { Exact = true }).FillAsync(username);
            await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
            await Page.GetByLabel("Password", new() { Exact = true }).FillAsync(password);
            await Page.GetByLabel("Confirm Password").ClickAsync();
            await Page.GetByLabel("Confirm Password").FillAsync(password);
            await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();

            // Assert
            // Should be logged in
            await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = $"What's on your mind {username}?" })).ToBeVisibleAsync();

            // Act
            // Post Cheep
            await Page.Locator("#MessageInput").ClickAsync();
            await Page.Locator("#MessageInput").FillAsync(message);
            await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

            // Assert
            // Cheep should be visible
            var postLocator = Page.Locator("li").Filter(new() { HasText = $"{username}" }).First;
            await Expect(postLocator).ToHaveTextAsync(new Regex($".*{message}.*"));

            // Act
            // Follow
            var posterToFollow = Page.Locator(".author").Nth(2);
            var posterToFollowUsername = await posterToFollow.TextContentAsync();
            await posterToFollow.ClickAsync();
            await Page.GetByRole(AriaRole.Button, new() { Name = "Follow" }).ClickAsync();


            // Assert
            // Should be following
            await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Unfollow" })).ToBeVisibleAsync();


            // Act
            // Unfollow

            await Page.GetByRole(AriaRole.Button, new() { Name = "Unfollow" }).ClickAsync();

            // Assert
            // Should be unfollowed
            await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Follow" })).ToBeVisibleAsync();

            // Act
            // Like a post
            await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
            var postToLike = Page.Locator("li").Filter(new() { HasText = "♡" }).First;
            var likeButton = postToLike.GetByRole(AriaRole.Button, new() { Name = "♡" });
            await likeButton.ClickAsync();

            // Assert
            // Should be liked
            await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "♥" })).ToBeVisibleAsync();
            await Expect(postToLike).ToHaveTextAsync(new Regex(@".*1.*"));

            // Act
            // Unlike a post
            await Page.GetByRole(AriaRole.Button, new() { Name = "♥" }).ClickAsync();

            // Assert
            // Should be unliked
            await Expect(postToLike.GetByRole(AriaRole.Button, new() { Name = "♡" })).ToBeVisibleAsync();
            await Expect(postToLike).ToHaveTextAsync(new Regex(@".*0.*"));

            // Act  
            // Logout

            await Page.GetByRole(AriaRole.Link, new() { Name = $"logout [{username}]" }).ClickAsync();

            // Assert
            // Should be logged out
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "login" })).ToBeVisibleAsync();
        }
    }
    
}