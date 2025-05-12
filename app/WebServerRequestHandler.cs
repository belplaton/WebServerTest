namespace waterb.app;

public abstract class WebServerRequestHandler<
    TWebServerRequestComposer, TWebServerRequestHandler> :
    IWebServerRequestHandler<TWebServerRequestComposer, TWebServerRequestHandler>
    where TWebServerRequestComposer : WebServerRequestComposer<TWebServerRequestComposer, TWebServerRequestHandler>
    where TWebServerRequestHandler : WebServerRequestHandler<TWebServerRequestComposer, TWebServerRequestHandler>
{
    public abstract string Pattern { get; }
    public abstract string HandleRequest(HttpContext context);
    public abstract void Initialize(WebServer server);
}