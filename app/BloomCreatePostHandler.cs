using System.Text;
using System.Text.Json;

namespace waterb.app;

public sealed class BloomCreatePostHandler : WebServerPostHandler
{
    private WebServer? _webServer;
    
    public override string Pattern => "/bloom/create";
    public override void HandleRequest(HttpRequest request, out string response, out int statusCode)
    {
        var data = JsonSerializer.Deserialize<BloodCreateData>(new StreamReader(request.Body).ReadToEnd());
        if (data == null)
        {
            response = "Invalid request";
            statusCode = 400;
            return;
        }

        var isAnyFailedHashCode = false;
        var failedHashCodesSb = new StringBuilder();
        var hashFunctions = data.Hashes.SelectMany(
            hashName =>
            {
                if (!IHashFunction.TryGetHashFunctions<string>(hashName, out var temp))
                {
                    failedHashCodesSb.Append($"{hashName}, ");
                    isAnyFailedHashCode = true;
                    return [];
                }

                return temp!.Select(hash => (IHashFunction<string>)hash);
            }).ToArray();

        if (isAnyFailedHashCode)
        {
            response = $"Can`t find hash functions for this request: {failedHashCodesSb}";
            statusCode = 404;
            return;
        }

        var bloomFilter = new BloomFilter<string>(data.Size, hashFunctions);

        _webServer!.AddService(bloomFilter);
        response = "OK";
        statusCode = 200;
    }

    public override void Initialize(WebServer? server)
    {
        _webServer = server; 
    }
    
    private record BloodCreateData(int Size, int HashCount, string[] Hashes);
}