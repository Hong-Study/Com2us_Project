public class TokenCheckMiddleware
{
    private readonly RequestDelegate _next;
    IMemoryRepository _memoryRepo;
    public TokenCheckMiddleware(RequestDelegate next, IMemoryRepository memoryRepo)
    {
        _next = next;
        _memoryRepo = memoryRepo;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.Value == "/api/login")
        {
            await _next(context);
        }
        else if (!context.Request.Headers.ContainsKey("Authorization") ||
                !context.Request.Headers.ContainsKey("UserId"))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Token And UserId Not Found");
            return;
        }
        

        string token = context.Request.Headers["Authorization"]!;
        string id = context.Request.Headers["UserId"]!;

        string? accessToken = await _memoryRepo.GetAccessToken(id);
        if (id == null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Token Not Found");
            return;
        }

        if(token != accessToken)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Token Not Match");
            return;
        }

        // context.Request.Headers["UserId"] = id;
        await _next(context);
    }
}