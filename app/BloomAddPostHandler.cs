using System.Text.Json;

namespace waterb.app;

public sealed class BloomAddPostHandler : WebServerPostHandler
{
    private WebServer? _server;
    public override string Pattern => "/bloom/add";

    public override async Task<WebServerRequestResponse> HandleRequest(HttpRequest request)
    {
        var dataPayload = await new StreamReader(request.Body).ReadToEndAsync();
        BloodAddData? data;
        try
        {
            data = JsonSerializer.Deserialize<BloodAddData>(dataPayload);
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
                response = $"Bloom filter with name \"{data.filterName}\" is not found.",
                statusCode = 404
            };
        }

        filter!.Add(data.value);
        return new WebServerRequestResponse
        {
            response = $"Successfully added new value \"{data.value}\" to bloom filter with name \"{data.filterName}\".", 
            statusCode = 200
        };
    }

    public override void Initialize(WebServer? server) => _server = server;

    private record BloodAddData(string filterName, string value);
}