namespace waterb.app;

public sealed class PingRequestHandler : IWebServerRequestHandler
{
    public string Pattern => "/ping";
    public string Get() => "pong!";
    
    public void Initialize(WebServer server) {}
}