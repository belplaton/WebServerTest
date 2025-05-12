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
            server.RegisterRequestHandler<DefaultGetHandler>();
            server.RegisterRequestHandler<PingGetHandler>();
            
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