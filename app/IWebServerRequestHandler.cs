namespace waterb.app;

public struct WebServerRequestResponse
{
    public string response;
    public int statusCode;
}

public interface IWebServerRequestHandler
{
    public string Pattern { get; }
    public Task<WebServerRequestResponse> HandleRequest(HttpRequest request);
    public void Initialize(WebServer? server);
}

public interface IWebServerRequestHandler<
    TWebServerRequestComposer, TWebServerRequestHandler> : IWebServerRequestHandler
    where TWebServerRequestComposer : WebServerRequestComposer<TWebServerRequestComposer, TWebServerRequestHandler>
    where TWebServerRequestHandler : WebServerRequestHandler<TWebServerRequestComposer, TWebServerRequestHandler>
{

}