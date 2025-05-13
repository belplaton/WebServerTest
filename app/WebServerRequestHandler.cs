namespace waterb.app;

public abstract class WebServerRequestHandler<
    TWebServerRequestComposer, TWebServerRequestHandler> :
    IWebServerRequestHandler<TWebServerRequestComposer, TWebServerRequestHandler>
    where TWebServerRequestComposer : WebServerRequestComposer<TWebServerRequestComposer, TWebServerRequestHandler>
    where TWebServerRequestHandler : WebServerRequestHandler<TWebServerRequestComposer, TWebServerRequestHandler>
{
    public abstract string Pattern { get; }
    public abstract Task<WebServerRequestResponse> HandleRequest(HttpRequest request);
    public abstract void Initialize(WebServer? server);
}