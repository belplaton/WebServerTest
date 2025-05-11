
namespace waterb.app;

public class WebServer : IDisposable
{
    private readonly WebApplication  _app;
    private bool _isListeningToCancelKeyPress;

    private readonly Dictionary<Type, string> _registeredRequestHandlers = new();
    private readonly Dictionary<string, WebServerRequestComposer> _registeredRequestComposers = new();

    public bool IsStopOnCancelKeyPressed
    {
        get => _isListeningToCancelKeyPress;
        set
        {
            if (_isListeningToCancelKeyPress == value) return;
            _isListeningToCancelKeyPress = value;
            if (value) Console.CancelKeyPress += OnCancelKeyPressed;
            else Console.CancelKeyPress -= OnCancelKeyPressed;
        }
    }
    
    public WebServer(params string[] urls)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost
            .UseUrls(urls)
            .ConfigureServices(services =>
            {
                // опционально: services.AddEndpointsApiExplorer();
            });

        _app = builder.Build();
    }

    public void RegisterRequestHandler<TWebServerRequestHandler>()
        where TWebServerRequestHandler : class, IWebServerRequestHandler, new()
    {
        var type = typeof(TWebServerRequestHandler);
        if (_registeredRequestHandlers.ContainsKey(type)) return;
        
        var handler = new TWebServerRequestHandler();
        _registeredRequestHandlers[type] = handler.Pattern;
        
        if (!_registeredRequestComposers.TryGetValue(handler.Pattern, out var composer))
        {
            composer = new WebServerRequestComposer();
            _registeredRequestComposers[handler.Pattern] = composer;
        }
            
        composer.AddHandler(handler);
    }
    
    public void UnregisterRequestHandler<TWebServerRequestHandler>()
        where TWebServerRequestHandler : class, IWebServerRequestHandler, new()
    {
        var type = typeof(TWebServerRequestHandler);
        if (!_registeredRequestHandlers.Remove(type, out var pattern)) return;
        
        var composer = _registeredRequestComposers[pattern];
        if (composer.RemoveHandler<TWebServerRequestHandler>(out var handler) && composer.HandlersCount == 0)
        {
            _registeredRequestComposers.Remove(pattern);
        }
    }

    public Task StartAsync()
    {
        var task = _app.StartAsync();
        Console.WriteLine($"Server start, listening URLs: {string.Join(", ", _app.Urls)}");
        return task;
    }

    public Task StopAsync()
    {
        var task = _app.StopAsync();
        IsStopOnCancelKeyPressed = false;
        Console.WriteLine("Server stopped.");
        return task;
    }
    
    /*
    
    private static async Task ProcessRequestAsync(HttpListenerContext ctx)
    {
        var path = ctx.Request.Url?.AbsolutePath.TrimEnd('/').ToLowerInvariant();
        var responseString = path == "/ping" ? "pong" : "Hello, world!";

        var buffer = Encoding.UTF8.GetBytes(responseString);
        ctx.Response.ContentLength64 = buffer.Length;
        ctx.Response.ContentType = "text/plain; charset=utf-8";

        await ctx.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        ctx.Response.Close();

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {ctx.Request.HttpMethod} {ctx.Request.Url} -> {responseString}");
    }
    
    */

    public void Dispose()
    {
        StopAsync().Wait();
    }

    private void OnCancelKeyPressed(object? sender, ConsoleCancelEventArgs e)
    {
        Console.WriteLine("Completion signal is received - shutting down...");
        StopAsync().Wait();
        e.Cancel = true;
    }
}