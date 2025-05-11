using System.Net;
using System.Text;

namespace waterb.app;

public class WebServer : IDisposable
{
    private readonly HttpListener _listener;
    private readonly CancellationTokenSource _cts = new();
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
    
    public WebServer(string prefix)
    {
        if (!HttpListener.IsSupported)
            throw new NotSupportedException("HttpListener is not supported on this platform.");

        _listener = new HttpListener();
        _listener.Prefixes.Add(prefix);
    }
    
    public async Task StartAsync()
    {
        _listener.Start();
        Console.WriteLine($"Server start, listening: {string.Join(", ", _listener.Prefixes)}");

        try
        {
            while (!_cts.IsCancellationRequested)
            {
                var context = await _listener.GetContextAsync().ConfigureAwait(false);
                _ = ProcessRequestAsync(context); // fire-and-forget
            }
        }
        catch (HttpListenerException) when (_cts.IsCancellationRequested)
        {

        }
    }
    
    public void Stop()
    {
        IsStopOnCancelKeyPressed = false;
        _cts.Cancel();
        _listener.Stop();
        Console.WriteLine("Server stopped.");
    }
    
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

    public void Dispose()
    {
        Stop();
        ((IDisposable)_listener).Dispose();
        _cts.Dispose();
    }

    private void OnCancelKeyPressed(object? sender, ConsoleCancelEventArgs e)
    {
        Console.WriteLine("Completion signal is received - shutting down...");
        Stop();
        e.Cancel = true;
    }
}