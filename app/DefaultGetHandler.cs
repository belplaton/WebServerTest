namespace waterb.app;

public sealed class DefaultGetHandler : WebServerGetHandler
{
    public override string Pattern => "/";
    public override Task<WebServerRequestResponse> HandleRequest(HttpRequest request)
    {
        var response = new WebServerRequestResponse
        {
            response = "Hello World!",
            statusCode = 200
        };
        
        return Task.FromResult(response);
    }

    public override void Initialize(WebServer? server) {}
}