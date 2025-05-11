using System.Net;

namespace waterb.app;

internal static class Program
{
    private static void Main(string[] args)
    {
        var prefix = args.Length > 0
            ? args[0]
            : "http://localhost:8080/";

        WebServer? server = null;
        try
        {
            server = new WebServer(prefix);
            server.IsStopOnCancelKeyPressed = true;
            server.StartAsync().GetAwaiter().GetResult();
        }
        catch (HttpListenerException)
        {
            
        }
        finally
        {
            server?.Dispose();
            Console.WriteLine("Server stopped.");
        }
    }
}