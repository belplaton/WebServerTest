namespace waterb.app;

public interface IWebServerRequestHandler
{
    public string Pattern { get; }
    public string Get();
}