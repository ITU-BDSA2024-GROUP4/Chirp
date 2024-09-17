using Microsoft.VisualStudio.TestPlatform.TestHost;
using Chirp.CLI;
using System;
using System.Diagnostics;
using System.ComponentModel;

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

	[Fact]
	public void End2End1(){
		using (var process = new Process())
		{
			process.StartInfo.FileName = "usr/bin/dotnet";
			process.StartInfo.Arguments = "./src/Chirp.CLI/bin/Debug/net7.0/Chirp.CLI.dll read 10";
			process.Start();
			Assert.Starts

		}


	}
}