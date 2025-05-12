using System.Text;

namespace waterb.app;

public sealed class WebServerPostComposer : WebServerRequestComposer<WebServerPostComposer, WebServerPostHandler>
{
    protected override async Task ComposeAsyncInternal(HttpContext context)
    {
        var sb = new StringBuilder();
        var method = context.Request.Method;            
        var path   = context.Request.Path + context.Request.QueryString;
        sb.AppendLine($"[{DateTime.Now:HH:mm:ss}] {method} {path} ->");
        using var enumerator = Handlers.GetEnumerator();
        while (enumerator.MoveNext())
        {
            string current;
            try
            {
                current = enumerator.Current.Value.HandleRequest(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                current = $"Error: {ex.Message}";
            }

            sb.AppendLine($"-> {current}");
        }

        await context.Response.WriteAsync(sb.ToString());
    }

    public override void Initialize(string pattern, WebServer server, WebApplication application)
    {
        application.MapPost(pattern, ComposeAsync);
    }
}