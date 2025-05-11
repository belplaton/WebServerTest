using System.Text;

namespace waterb.app;

public sealed class WebServerRequestComposer
{
    private readonly Dictionary<Type, IWebServerRequestHandler> _handlers = [];

    public string Compose(HttpContext context)
    {
        var sb = new StringBuilder();
        var method = context.Request.Method;            
        var path   = context.Request.Path + context.Request.QueryString;
        sb.AppendLine($"[{DateTime.Now:HH:mm:ss}] {method} {path} ->");
        using var enumerator = _handlers.GetEnumerator();
        while (enumerator.MoveNext())
        {
            string current;
            try
            {
                current = enumerator.Current.Value.Get();
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                current = $"500: {ex.Message}";
            }

            sb.AppendLine($"-> {current}");
        }
        
        return sb.ToString();
    }
    
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