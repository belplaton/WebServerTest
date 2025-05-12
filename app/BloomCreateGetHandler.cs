namespace waterb.app;

public sealed class BloomCreateGetHandler : WebServerGetHandler
{
    public string Pattern => "/bloom/create";
    public string Get() => throw new NotSupportedException();
    public void Initialize(WebServer server) { /* здесь можно сохранять сервер */ }
    
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