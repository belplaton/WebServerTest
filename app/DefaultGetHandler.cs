namespace waterb.app;

public sealed class DefaultGetHandler : WebServerGetHandler
{
    public override string Pattern => "/";
    public override void HandleRequest(HttpRequest request, out string response, out int statusCode)
    {
        response = "Hello World!";
        statusCode = 200;
    }

    public override void Initialize(WebServer server) {}
}