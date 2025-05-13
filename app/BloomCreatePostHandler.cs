using System.Text;
using System.Text.Json;

namespace waterb.app;

public sealed class BloomCreatePostHandler : WebServerPostHandler
{
    private WebServer? _webServer;
    
    public override string Pattern => "/bloom/create";
    public override async Task<WebServerRequestResponse> HandleRequest(HttpRequest request)
    {
        var dataPayload = await new StreamReader(request.Body).ReadToEndAsync();
        BloomCreateData? data;
        try
        {
            data = JsonSerializer.Deserialize<BloomCreateData>(dataPayload);
        }
        catch
        {
            data = null;
        }

        if (data is not { size: > 0 } || string.IsNullOrEmpty(data.filterName) || data.hashes.Length == 0)
        {
            return new WebServerRequestResponse
            {
                response = "Invalid request. Please follow this form:\n" +
                   "{\n" +
                   "\t\"filterName:\" <string>,\n" +
                   "\t\"size:\" <int>,\n" +
                   "\t\"hashes\": [...<string>]\n" +
                   "}",
                statusCode = 400
            };
        }

        var isAnyFailedHashCode = false;
        var failedHashCodesSb = new StringBuilder();
        var hashFunctions = data.hashes.SelectMany(
            hashName =>
            {
                if (!IHashFunction.CreateHashFunctions<string>(hashName, out var temp))
                {
                    failedHashCodesSb.Append($"{hashName}, ");
                    isAnyFailedHashCode = true;
                    return [];
                }

                return temp!;
            }).ToArray();

        if (isAnyFailedHashCode)
        {
            return new WebServerRequestResponse
            {
                response = $"Can`t find hash functions for this request: {failedHashCodesSb}",
                statusCode = 404
            };
        }
        
        var bloomFilter = new BloomFilter<string>(data.size, hashFunctions);
        if (!_webServer!.TryGetService<BloomFilterProvider<string>>(out var provider))
        {
            provider = new BloomFilterProvider<string>();
            _webServer.AddService(provider);
        }
        
        var isOverride = provider!.TryGet(data.filterName, out _);
        provider.Set(data.filterName, bloomFilter);
        return new WebServerRequestResponse
        {
            response = $"Bloom filter with name \"{data.filterName}\" " +
                $"was successfully {(isOverride ? "override" : "created")}.", 
            statusCode = 201
        };
    }

    public override void Initialize(WebServer? server)
    {
        _webServer = server; 
    }
    
    private record BloomCreateData(string filterName, int size, string[] hashes);
}