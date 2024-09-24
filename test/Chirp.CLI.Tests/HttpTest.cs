using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Chirp.CLI.Tests;

public static class HttpTest
{
    public const string BASEURL = "http://localhost:5141";
    
    [Fact]
    public static async void ReadCheepsTest()
    {
        using HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri(BASEURL);
        
        var response = await client.GetAsync("/cheeps");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        string responseString = await response.Content.ReadAsStringAsync();
            
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        Assert.Equal(typeof(List<Cheep>), JsonSerializer.Deserialize<List<Cheep>>(responseString, options).GetType());
    }

    [Fact]
    public static async void WriteCheepsTest()
    {
        using HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri(BASEURL);
        
        DateTimeOffset timeStamp = DateTime.UtcNow;
        
        Cheep cheep = new Cheep("Sigma", "Hop ind i bilen", timeStamp.ToUnixTimeSeconds());

        JsonContent jsonCheep = JsonContent.Create(cheep);
        
        var response =  await client.PostAsync(BASEURL + "/cheep", jsonCheep);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}