namespace waterb.app;

public class WebServerRequestComposer
{
    private readonly Dictionary<Type, IWebServerRequestHandler> _handlers = [];
    
    public string Compose() => string.Join("\n", _handlers.Select(h => h.Value.Get()));
    
    public void AddHandler<TWebServerRequestHandler>(TWebServerRequestHandler handler) 
        where TWebServerRequestHandler : class, IWebServerRequestHandler, new() =>
        _handlers.Add(typeof(TWebServerRequestHandler), handler);
    public bool RemoveHandler<TWebServerRequestHandler>()
        where TWebServerRequestHandler : class, IWebServerRequestHandler, new() => 
        _handlers.Remove(typeof(TWebServerRequestHandler));

    public bool RemoveHandler<TWebServerRequestHandler>(out TWebServerRequestHandler? handler)
        where TWebServerRequestHandler : class, IWebServerRequestHandler, new()
    {
        if (_handlers.Remove(typeof(TWebServerRequestHandler), out var temp))
        {
            handler = (TWebServerRequestHandler)temp;
            return true;
        }

        handler = default;
        return false;
    }
    
    public int HandlersCount => _handlers.Count;
}