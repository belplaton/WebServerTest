namespace waterb.app;

public sealed class PingGetHandler : WebServerGetHandler
{
    public override string Pattern => "/ping";
    public override string HandleRequest(HttpContext context) => "pong!";
    public override void Initialize(WebServer server) {}
}