namespace waterb.app;

public sealed class WebServer : IDisposable
{
    private readonly WebApplication _app;
    private bool _isListeningToCancelKeyPress;

    private readonly Dictionary<Type, string> _registeredRequestHandlers = new();
    private readonly Dictionary<string,
        Dictionary<Type, IWebServerRequestComposer>> _registeredRequestComposers = new();

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

    public void RegisterRequestHandler<
        TWebServerRequestComposer, TWebServerRequestHandler, MWebServerRequestHandler>()
        where TWebServerRequestComposer : WebServerRequestComposer<
            TWebServerRequestComposer, TWebServerRequestHandler>, new()
        where TWebServerRequestHandler : WebServerRequestHandler<
            TWebServerRequestComposer, TWebServerRequestHandler>
        where MWebServerRequestHandler : TWebServerRequestHandler, new()
    {
        var handlerType = typeof(TWebServerRequestHandler);
        var composerType = typeof(TWebServerRequestComposer);
        if (_registeredRequestHandlers.ContainsKey(handlerType)) return;
        
        var handler = new MWebServerRequestHandler();
        handler.Initialize(this);
        _registeredRequestHandlers[handlerType] = handler.Pattern;
        
        if (!_registeredRequestComposers.TryGetValue(handler.Pattern, out var composers))
        {
            composers = new Dictionary<Type, IWebServerRequestComposer>();
            _registeredRequestComposers[handler.Pattern] = composers;
        }

        TWebServerRequestComposer composer;
        if (!composers.TryGetValue(composerType, out var composerRaw))
        {
            composer = new TWebServerRequestComposer();
            composer.Initialize(handler.Pattern, this, _app);
            composers[composerType] = composerRaw = composer;
        }
        else composer = (TWebServerRequestComposer)composerRaw;
        
        composer.AddHandler(handler);
    }
    
    public void UnregisterRequestHandler<
        TWebServerRequestComposer, TWebServerRequestHandler, MWebServerRequestHandler>()
        where TWebServerRequestComposer : WebServerRequestComposer<
            TWebServerRequestComposer, TWebServerRequestHandler>, new()
        where TWebServerRequestHandler : WebServerRequestHandler<
            TWebServerRequestComposer, TWebServerRequestHandler>
        where MWebServerRequestHandler : TWebServerRequestHandler, new()
    {
        var handlerType = typeof(TWebServerRequestHandler);
        var composerType = typeof(TWebServerRequestComposer);
        if (!_registeredRequestHandlers.Remove(handlerType, out var pattern)) return;
        
        var composers = _registeredRequestComposers[pattern];
        var composer = (TWebServerRequestComposer)composers[composerType];
        
        composer.RemoveHandler<MWebServerRequestHandler>();
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