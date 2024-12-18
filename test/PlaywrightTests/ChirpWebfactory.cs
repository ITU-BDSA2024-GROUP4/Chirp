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
            _process.StartInfo.CreateNoWindow = true;
            
            //Thread.Sleep(10000);
            
        }

        public void Start()
        {
            _process.Start();
            WaitForIsStarted();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _process.Kill();
            _process.Dispose();
            var ChirpWebProcess = Process.GetProcessesByName("Chirp.Web");
            if (ChirpWebProcess.Any())
            {
                ChirpWebProcess.ToList().ForEach(process => process.Kill());
            }
        }

        public string GetBaseAddress()
        {
            return _baseAddress;
        }

        public void WaitForIsStarted()
        {
            var _client = new HttpClient();
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    var result = _client.GetAsync($"{_baseAddress}").Result;

                    if (result.IsSuccessStatusCode)
                    {
                        return;
                    }
                }
                catch
                {
                   
                }

                Thread.Sleep(3000);
            }

            throw new Exception("Could not connect to server.");
        }
    }
}