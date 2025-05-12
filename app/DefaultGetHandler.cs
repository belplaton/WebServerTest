namespace waterb.app;

public sealed class DefaultGetHandler : WebServerGetHandler
{
    public override string Pattern => "/";
    public override string HandleRequest(HttpContext context) => "Hello, world!";
    public override void Initialize(WebServer server) {}
}