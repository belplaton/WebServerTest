namespace waterb.app;

public sealed class DefaultGetHandler : WebServerGetHandler
{
    public string Pattern => "/";
    public string Get() => "Hello, world!";
    
    public void Initialize(WebServer server) {}
}