namespace waterb.app;

public sealed class PingGetHandler : WebServerGetHandler
{
    public string Pattern => "/ping";
    public string Get() => "pong!";
    
    public void Initialize(WebServer server) {}
}