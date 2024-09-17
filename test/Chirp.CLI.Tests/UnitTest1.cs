using Microsoft.VisualStudio.TestPlatform.TestHost;
using Chirp.CLI;

namespace Chirp.CLI.Tests;



public class UnitTest1
{
    [Fact]
    public void DateTimeTest()
    {
        var UI = new UserInterface();
        DateTimeOffset TimeStamp = DateTime.UtcNow;
        Assert.Equal(UI.ConvertTime(TimeStamp.ToUnixTimeSeconds()).ToString(), TimeStamp.ToLocalTime().ToString());
    }
}