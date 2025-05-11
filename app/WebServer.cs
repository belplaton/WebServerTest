
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
        builder.WebHost.UseUrls(urls);

        _app = builder.Build();
    }

    public void RegisterRequestHandler<TWebServerRequestHandler>()
        where TWebServerRequestHandler : class, IWebServerRequestHandler, new()
    {
        var type = typeof(TWebServerRequestHandler);
        if (_registeredRequestHandlers.ContainsKey(type)) return;
        
        var handler = new TWebServerRequestHandler();
        handler.Initialize(this);
        _registeredRequestHandlers[type] = handler.Pattern;
        
        if (!_registeredRequestComposers.TryGetValue(handler.Pattern, out var composer))
        {
            composer = new WebServerRequestComposer();
            _registeredRequestComposers[handler.Pattern] = composer;
            _app.MapGet(handler.Pattern, composer.Compose);
        }
            
        composer.AddHandler(handler);
    }
    
    public void UnregisterRequestHandler<TWebServerRequestHandler>()
        where TWebServerRequestHandler : class, IWebServerRequestHandler, new()
    {
        var type = typeof(TWebServerRequestHandler);
        if (!_registeredRequestHandlers.Remove(type, out var pattern)) return;
        
        var composer = _registeredRequestComposers[pattern];
        composer.RemoveHandler<TWebServerRequestHandler>();
    }

    public Task StartAsync()
    {
        var task = _app.StartAsync();
        Console.WriteLine($"Server start, listening URLs: {string.Join(", ", _app.Urls)}");
        return task;
    }
    
    public Task RunAsync()
    {
        var task = _app.RunAsync();
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
    
    public void Dispose()
    {
        _app.DisposeAsync().GetAwaiter().GetResult();
    }

    private void OnCancelKeyPressed(object? sender, ConsoleCancelEventArgs e)
    {
        Console.WriteLine("Completion signal is received - shutting down...");
        StopAsync().Wait();
        e.Cancel = true;
    }
}