namespace waterb.app;

public interface IWebServerRequestHandler
{
    public string Pattern { get; }
    public string Get();
    
    public void Initialize(WebServer server);
}