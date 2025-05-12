namespace waterb.app;

public sealed class BloomCreateGetHandler : WebServerPostHandler
{
    private WebServer _webServer;
    
    public override string Pattern => "/bloom/create";
    public override string HandleRequest(HttpContext context)
    {
        
    }
    
    public override void Initialize(WebServer server) {}
    
    
    public void Post(HttpContext ctx)
    {
        var req = JsonSerializer.Deserialize<InitDto>(new StreamReader(ctx.Request.Body).ReadToEnd());
        // собрать массив функций по именам из req.hashes
        BloomGlobal.Instance = new BloomFilter<string>(
            req.size,
            req.hashes.Select(CreateHash).ToArray()
        );
        ctx.Response.StatusCode = 200;
        ctx.Response.WriteAsync("OK");
    }
}