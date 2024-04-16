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
        if(context.Request.Path.Value == "/api/login")
        {
            await _next(context);
        }
        else if (context.Request.Headers.ContainsKey("Authorization"))
        {
            string? token = context.Request.Headers["Authorization"];
            if(token == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token Not Found");
                return;
            }

            string? memoryToken = await _memoryRepo.GetAccessToken(token);
            if(memoryToken == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token Not Found");
                return;
            }

            await _next(context);
        }
        else
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Token Not Found");
        }
    }
}