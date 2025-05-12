namespace waterb.app;

public interface IWebServerRequestComposer
{
    public Task ComposeAsync(HttpContext context);
    public void Initialize(string pattern, WebServer server, WebApplication application);

    public void AddHandler(IWebServerRequestHandler handler);
    public bool RemoveHandler(Type handlerType);
    public bool RemoveHandler(Type handlerType, out IWebServerRequestHandler? handler);
    public int HandlersCount { get; }
}

public interface IWebServerRequestComposer<
    TWebServerRequestComposer, TWebServerRequestHandler> : IWebServerRequestComposer
    where TWebServerRequestComposer : WebServerRequestComposer<TWebServerRequestComposer, TWebServerRequestHandler>
    where TWebServerRequestHandler : WebServerRequestHandler<TWebServerRequestComposer, TWebServerRequestHandler>
{

}