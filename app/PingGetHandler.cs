namespace waterb.app;

public sealed class PingGetHandler : WebServerGetHandler
{
    public override string Pattern => "/ping";

    public override void HandleRequest(HttpRequest request, out string response, out int statusCode)
    {
        response = "pong!";
        statusCode = 200;
    }
    
    public override void Initialize(WebServer? server) {}
}