using System.Diagnostics;
using SimpleDB;

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
		    process.StartInfo.FileName = "../../../../../src/Chirp.CLI/bin/Debug/net7.0/linux-x64/Chrip.CLI";
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
	    using (var process = new Process())
	    {
		    process.StartInfo.FileName = "../../../../../src/Chirp.CLI/bin/Debug/net7.0/linux-x64/Chrip.CLI";
		    process.StartInfo.WorkingDirectory = "../../../../";
		    process.StartInfo.Arguments = "cheep \"Hello!!!\"";
		    process.Start();
		    
		    process.WaitForExit();
	    }
        
	    var read = CSVDatabase<Cheep>.Instance.Read(1);

	    foreach (Cheep cheep in read)
	    {
		    Assert.Equal("Hello!!!", cheep.Message);
	    }
    }
}