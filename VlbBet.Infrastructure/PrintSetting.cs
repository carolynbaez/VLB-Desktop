using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

public static class PrintModule
{
    private static readonly HttpClient _http = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5000/")
    };

    private static readonly JsonSerializerOptions _json = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static async Task ToPrintAsync(object data, CancellationToken ct = default)
    {
        var body = JsonSerializer.Serialize(data, _json);
        
        using var content = new StringContent(body, Encoding.UTF8, "application/json");

        using var resp = await _http.PostAsync("print", content, ct);

        resp.EnsureSuccessStatusCode();
    }
}

public class PrintObject
{
    public object data { get; set; }
    public string Type { get; set; }
    public string logo { get; set; }
}