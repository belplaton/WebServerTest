using System.Text.Json;

namespace waterb.app;

public sealed class BloomRemovePostHandler : WebServerPostHandler
{
    private WebServer? _server;
    public override string Pattern => "/bloom/remove";

    public override async Task<WebServerRequestResponse> HandleRequest(HttpRequest request)
    {
        var dataPayload = await new StreamReader(request.Body).ReadToEndAsync();
        BloomRemoveData? data;
        try
        {
            data = JsonSerializer.Deserialize<BloomRemoveData>(dataPayload);
        }
        catch
        {
            data = null;
        }

        if (data == null || string.IsNullOrEmpty(data.filterName))
        {
            return new WebServerRequestResponse
            {
                response = "Invalid request. Please follow this form:\n" +
                   "{\n" +
                   "\t\"filterName:\" <string>\n" +
                   "}",
                statusCode = 400
            };
        }

        if (!_server!.TryGetService<BloomFilterProvider<string>>(out var provider) ||
            !provider!.TryRemove(data.filterName, out _))
        {
            return new WebServerRequestResponse
            {
                response = $"Bloom filter with name {data.filterName} is not found.",
                statusCode = 404
            };
        }
        
        return new WebServerRequestResponse
        {
            response = $"Bloom filter with name \"{data.filterName}\" is successfully removed.",
            statusCode = 200
        };
    }

    public override void Initialize(WebServer? server) => _server = server;

    private record BloomRemoveData(string filterName);
}