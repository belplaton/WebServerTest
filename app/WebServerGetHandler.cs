namespace waterb.app;

public abstract class WebServerGetHandler : WebServerRequestHandler<WebServerGetComposer, WebServerGetHandler>
{
    public override string HandleRequest(HttpContext context)
    {
        throw new NotImplementedException();
    }

    public override void Initialize(WebServer server)
    {
        throw new NotImplementedException();
    }
}