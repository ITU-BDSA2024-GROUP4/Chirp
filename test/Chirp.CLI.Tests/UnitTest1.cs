using System.Diagnostics;
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

    [Fact]
    public void End2End1()
    {
	    int lineCounter = 0;
	    
	    using (var process = new Process())
	    {
		    process.StartInfo.FileName = "../../../../../src/Chirp.CLI/bin/Debug/net8.0/linux-x64/Chirp.CLI";
		    process.StartInfo.WorkingDirectory = "../../../../";
		    process.StartInfo.Arguments = "read 10";
		    process.StartInfo.RedirectStandardOutput = true;
		    process.Start();

		    while (!process.StandardOutput.EndOfStream)
		    {
			    process.StandardOutput.ReadLine();
			    lineCounter++;
		    }
		    
		    process.WaitForExit();
	    }

	    Assert.Equal(10, lineCounter);
    }

    [Fact]
    public void End2End2()
    {
        string output;
        
	    using (var process = new Process())
	    {
		    process.StartInfo.FileName = "../../../../../src/Chirp.CLI/bin/Debug/net8.0/linux-x64/Chirp.CLI";
		    process.StartInfo.WorkingDirectory = "../../../../";
		    process.StartInfo.Arguments = "cheep \"Hello!!!\"";
		    process.Start();
		    
		    process.WaitForExit();
            
            process.StartInfo.Arguments = "read 1";
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            
            output = process.StandardOutput.ReadToEnd();
            
            process.WaitForExit();
	    }

        output = output.Split(' ').Last().Trim();
        
        Assert.Equal("Hello!!!", output);
    }
}