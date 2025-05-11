namespace waterb.app;

public sealed class DefaultRequestHandler : IWebServerRequestHandler
{
    public string Pattern => "/";
    public string Get() => "Hello, world!";
    
    public void Initialize(WebServer server) {}
}