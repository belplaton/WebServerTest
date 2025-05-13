namespace waterb.app;

public sealed class WebServer : IDisposable
{
    private readonly WebApplication _app;
    public WebServer(params string[] urls)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls(urls);

        _app = builder.Build();
    }

    #region Services

    private readonly Dictionary<Type, IWebServerService> _services = new();
    public void AddService<TWebServerService>(TWebServerService service)
        where TWebServerService : class, IWebServerService
    {
        _services[typeof(TWebServerService)] = service;
    }

    public bool TryGetService<TWebServerService>(out TWebServerService? service)
        where TWebServerService : class, IWebServerService
    {
        if (_services.TryGetValue(typeof(TWebServerService), out var serviceRaw))
        {
            service = (TWebServerService)serviceRaw;
            return true;
        }

        service = null;
        return false;
    }
    
    public void RemoveService<TWebServerService>() where TWebServerService : class, IWebServerService
        => _services.Remove(typeof(TWebServerService));
    public void ClearServices() => _services.Clear();

    #endregion
    
    #region Request Handlers

    private readonly Dictionary<Type, string> _registeredRequestHandlers = new();
    private readonly Dictionary<string, Dictionary<Type, IWebServerRequestComposer>> 
        _registeredRequestComposers = new();
    
    public void RegisterRequestHandler<
        TWebServerRequestComposer, TBaseWebServerRequestHandler, TRealWebServerRequestHandler>()
        where TWebServerRequestComposer : WebServerRequestComposer<
            TWebServerRequestComposer, TBaseWebServerRequestHandler>, new()
        where TBaseWebServerRequestHandler : WebServerRequestHandler<
            TWebServerRequestComposer, TBaseWebServerRequestHandler>
        where TRealWebServerRequestHandler : TBaseWebServerRequestHandler, new()
    {
        var handlerType = typeof(TRealWebServerRequestHandler);
        var composerType = typeof(TWebServerRequestComposer);
        if (_registeredRequestHandlers.ContainsKey(handlerType)) return;
        
        var handler = new TRealWebServerRequestHandler();
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
            composers[composerType] = composer;
        }
        else composer = (TWebServerRequestComposer)composerRaw;
        
        composer.AddHandler(handler);
    }
    
    public void UnregisterRequestHandler<
        TWebServerRequestComposer, TBaseWebServerRequestHandler, TRealWebServerRequestHandler>()
        where TWebServerRequestComposer : WebServerRequestComposer<
            TWebServerRequestComposer, TBaseWebServerRequestHandler>, new()
        where TBaseWebServerRequestHandler : WebServerRequestHandler<
            TWebServerRequestComposer, TBaseWebServerRequestHandler>
        where TRealWebServerRequestHandler : TBaseWebServerRequestHandler, new()
    {
        var handlerType = typeof(TRealWebServerRequestHandler);
        var composerType = typeof(TWebServerRequestComposer);
        if (!_registeredRequestHandlers.Remove(handlerType, out var pattern)) return;
        
        var composers = _registeredRequestComposers[pattern];
        var composer = (TWebServerRequestComposer)composers[composerType];
        
        composer.RemoveHandler<TRealWebServerRequestHandler>();
    }

    #endregion
    
    #region Life Cycle
    
    private bool _isListeningToCancelKeyPress;
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
    
    #endregion
}