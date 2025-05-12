namespace waterb.app;

public interface IWebServerRequestHandler
{
    public string Pattern { get; }
    public string HandleRequest(HttpContext context);
    public void Initialize(WebServer server);
}

public interface IWebServerRequestHandler<
    TWebServerRequestComposer, TWebServerRequestHandler> : IWebServerRequestHandler
    where TWebServerRequestComposer : WebServerRequestComposer<TWebServerRequestComposer, TWebServerRequestHandler>
    where TWebServerRequestHandler : WebServerRequestHandler<TWebServerRequestComposer, TWebServerRequestHandler>
{

}