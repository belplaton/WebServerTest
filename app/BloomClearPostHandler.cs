using System.Text.Json;

namespace waterb.app;

public sealed class BloomClearPostHandler : WebServerPostHandler
{
    private WebServer? _server;
    public override string Pattern => "/bloom/clear";

    public override async Task<WebServerRequestResponse> HandleRequest(HttpRequest request)
    {
        var dataPayload = await new StreamReader(request.Body).ReadToEndAsync();
        BloomClearData? data;
        try
        {
            data = JsonSerializer.Deserialize<BloomClearData>(dataPayload);
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
            !provider!.TryGet(data.filterName, out var filter))
        {
            return new WebServerRequestResponse
            {
                response = $"Bloom filter with name {data.filterName} is not found.",
                statusCode = 404
            };
        }

        filter!.Clear();
        return new WebServerRequestResponse
        {
            response = $"Bloom filter with name \"{data.filterName}\" is successfully cleared.", 
            statusCode = 200
        };
    }

    public override void Initialize(WebServer? server) => _server = server;

    private record BloomClearData(string filterName);
}