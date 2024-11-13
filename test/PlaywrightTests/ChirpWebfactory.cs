using System.Diagnostics;

namespace PlaywrightTests
{
    public class ChirpWebfactory : IDisposable
    {
        private readonly Process _process = null!;
        private readonly string path = "../../../../../src/Chirp.Web";
        private readonly string _baseAddress;

        public ChirpWebfactory()
        {
            _baseAddress = "http://localhost:5273";
            _process = new Process();
            _process.StartInfo.FileName = "dotnet";
            _process.StartInfo.Arguments = "run --project " + path;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.RedirectStandardError = true;
            _process.StartInfo.UseShellExecute = false;

            Thread.Sleep(10000);
        }

        public void Start()
        {
            _ = _process.Start();
        }

        public void Dispose()
        {
            _process.Kill();
            _process.Dispose();
        }

        public string GetBaseAddress()
        {
            return _baseAddress;
        }


    }
}

