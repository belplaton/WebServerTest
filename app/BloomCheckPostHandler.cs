using System.Text.Json;

namespace waterb.app;

public sealed class BloomCheckPostHandler : WebServerPostHandler
{
    private WebServer? _server;
    public override string Pattern => "/bloom/check";

    public override async Task<WebServerRequestResponse> HandleRequest(HttpRequest request)
    {
        var dataPayload = await new StreamReader(request.Body).ReadToEndAsync();
        BloomCheckData? data;
        try
        {
            data = JsonSerializer.Deserialize<BloomCheckData>(dataPayload);
        }
        catch
        {
            data = null;
        }
        
        if (data == null)
        {
            return new WebServerRequestResponse
            {
                response = "Invalid request. Please follow this form:\n" +
                           "{\n" +
                           "\t\"filterName:\" <string>,\n" +
                           "\t\"value:\" <string>\n" +
                           "}",
                statusCode = 400
            };
        }

        if (!_server!.TryGetService<BloomFilterProvider<string>>(out var provider) ||
            !provider!.TryGet(data.filterName, out var filter))
        {
            return new WebServerRequestResponse
            {
                response = $"Bloom filter with name {data.filterName} is not found.",
                statusCode = 404
            };
        }

        return new WebServerRequestResponse
        {
            response = $"Bloom filter with name \"{data.filterName}\"" +
                       $"{(filter!.Contains(data.value) ? "is" : "is not")} contains {data.value}.", 
            statusCode = 200
        };
    }

    public override void Initialize(WebServer? server) => _server = server;

    private record BloomCheckData(string filterName, string value);
}