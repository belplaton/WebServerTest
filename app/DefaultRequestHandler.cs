namespace waterb.app;

public class DefaultRequestHandler : IWebServerRequestHandler
{
    public string Pattern => "/";
    public string Get() => "Hello, world!";
    
    public void Initialize(WebServer server) {}
}