namespace waterb.app;

public sealed class PingGetHandler : WebServerGetHandler
{
    public override string Pattern => "/ping";
    public override Task<WebServerRequestResponse> HandleRequest(HttpRequest request)
    {
        var response = new WebServerRequestResponse
        {
            response = "pong!",
            statusCode = 200
        };
        
        return Task.FromResult(response);
    }
    
    public override void Initialize(WebServer? server) {}
}