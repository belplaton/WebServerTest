using System.Net;

namespace waterb.app;

internal static class Program
{
    private static void Main(string[] args)
    {
        var prefix = args.Length > 0
            ? args[0]
            : "http://0.0.0.0:8080";

        WebServer? server = null;
        try
        {
            server = new WebServer(prefix);
            server.RegisterRequestHandler<WebServerGetComposer, WebServerGetHandler, DefaultGetHandler>();
            server.RegisterRequestHandler<WebServerGetComposer, WebServerGetHandler, PingGetHandler>();
            
            server.RegisterRequestHandler<WebServerPostComposer, WebServerPostHandler, BloomCreatePostHandler>();
            
            server.IsStopOnCancelKeyPressed = true;
            server.RunAsync().GetAwaiter().GetResult();
        }
        catch (HttpListenerException)
        {
            server?.StopAsync().Wait();
        }
        finally
        {
            server?.Dispose();
            Console.WriteLine("Server stopped.");
        }
    }
}