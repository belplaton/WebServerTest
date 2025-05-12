using System.Text;

namespace waterb.app;

public sealed class WebServerGetComposer : WebServerRequestComposer<WebServerGetComposer, WebServerGetHandler>
{
    protected override async Task ComposeAsyncInternal(HttpContext context)
    {
        var sb = new StringBuilder();
        var method = context.Request.Method;            
        var path   = context.Request.Path + context.Request.QueryString;
        int? lastStatusCode = null;
        var differentStatusCodeCount = 0;
        
        sb.AppendLine($"[{DateTime.Now:HH:mm:ss}] {method} {path} ->");
        using var enumerator = Handlers.GetEnumerator();
        while (enumerator.MoveNext())
        {
            string current;
            int statusCode;
            try
            {
                enumerator.Current.Value.HandleRequest(context.Request, out current, out statusCode);
            }
            catch (Exception ex)
            {
                current = $"Error: {ex.Message}";
                statusCode = 500;
            }
            
            sb.AppendLine($"-> [{statusCode}] {current}");
            if (lastStatusCode.HasValue && lastStatusCode.Value != statusCode) differentStatusCodeCount++;
            lastStatusCode = statusCode;
        }

        context.Response.StatusCode = differentStatusCodeCount == 0 ? lastStatusCode ?? 200 : 207;
        await context.Response.WriteAsync(sb.ToString());
    }

    public override void Initialize(string pattern, WebServer server, WebApplication application)
    {
        application.MapGet(pattern, ComposeAsync);
    }
}