namespace waterb.app;

public interface IWebServerRequestHandler
{
    public string Pattern { get; }
    public void HandleRequest(HttpRequest request, out string response, out int statusCode);
    public void Initialize(WebServer server);
}

public interface IWebServerRequestHandler<
    TWebServerRequestComposer, TWebServerRequestHandler> : IWebServerRequestHandler
    where TWebServerRequestComposer : WebServerRequestComposer<TWebServerRequestComposer, TWebServerRequestHandler>
    where TWebServerRequestHandler : WebServerRequestHandler<TWebServerRequestComposer, TWebServerRequestHandler>
{

}