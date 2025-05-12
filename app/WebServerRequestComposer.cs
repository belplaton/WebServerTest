namespace waterb.app;

public abstract class WebServerRequestComposer<
    TWebServerRequestComposer, TWebServerRequestHandler> :
    IWebServerRequestComposer<TWebServerRequestComposer, TWebServerRequestHandler>
    where TWebServerRequestComposer : WebServerRequestComposer<TWebServerRequestComposer, TWebServerRequestHandler>
    where TWebServerRequestHandler : WebServerRequestHandler<TWebServerRequestComposer, TWebServerRequestHandler>
{
    private readonly Dictionary<Type, IWebServerRequestHandler> _handlers = [];
    protected IReadOnlyDictionary<Type, IWebServerRequestHandler> Handlers => _handlers;
    public int HandlersCount => _handlers.Count;
    public Task ComposeAsync(HttpContext context) => 
        _handlers.Count > 0 ? ComposeAsyncInternal(context) : Task.CompletedTask;
    
    protected abstract Task ComposeAsyncInternal(HttpContext context);
    public abstract void Initialize(string pattern, WebServer server, WebApplication application);

    public void AddHandler(IWebServerRequestHandler handler)
    {
        var handlerType = handler.GetType();
        if (!handlerType.IsAssignableFrom(typeof(TWebServerRequestHandler))) throw new ArgumentException(
            $"Type '{handlerType}' does not implement '{typeof(TWebServerRequestHandler)}'.");
        _handlers.Add(handlerType, handler);
    }
    
    public bool RemoveHandler(Type handlerType)
    {
        if (!handlerType.IsAssignableFrom(typeof(TWebServerRequestHandler))) throw new ArgumentException(
            $"Type '{handlerType}' does not implement '{typeof(TWebServerRequestHandler)}'.");
        return _handlers.Remove(handlerType);
    }

    public bool RemoveHandler(Type handlerType, out IWebServerRequestHandler? handler)
    {
        if (!handlerType.IsAssignableFrom(typeof(TWebServerRequestHandler))) throw new ArgumentException(
            $"Type '{handlerType}' does not implement '{typeof(TWebServerRequestHandler)}'.");
        return _handlers.Remove(handlerType, out handler);
    }
    
    public void AddHandler<MWebServerRequestHandler>(MWebServerRequestHandler handler) 
        where MWebServerRequestHandler : class, TWebServerRequestHandler, new() =>
        _handlers.Add(typeof(MWebServerRequestHandler), handler);

    public bool RemoveHandler<MWebServerRequestHandler>()
        where MWebServerRequestHandler : class, TWebServerRequestHandler, new() => 
        _handlers.Remove(typeof(MWebServerRequestHandler));

    public bool RemoveHandler<MWebServerRequestHandler>(out MWebServerRequestHandler? handler)
        where MWebServerRequestHandler : class, TWebServerRequestHandler, new()
    {
        if (_handlers.Remove(typeof(MWebServerRequestHandler), out var temp))
        {
            handler = (MWebServerRequestHandler)temp;
            return true;
        }

        handler = default;
        return false;
    }
}