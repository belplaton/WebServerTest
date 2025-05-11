namespace waterb.app;

public class PingRequestHandler : IWebServerRequestHandler
{
    public string Pattern => "/ping";
    public string Get() => "pong!";
    
    public void Initialize(WebServer server) {}
}