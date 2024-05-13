namespace APIServer;

public class RequestOneCheckMiddleware
{
    private readonly RequestDelegate _next;
    RedisLockRepository _lockRepo;
    public RequestOneCheckMiddleware(RequestDelegate next, RedisLockRepository lockRepo)
    {
        _next = next;
        _lockRepo = lockRepo;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.Value == "/api/login")
        {
            await _next(context);
            return;
        }

        string id = context.Request.Headers["UserId"]!;

        if (await _lockRepo.LockAsync(id))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Request One Time");
            return;
        }

        await _next(context);

        if (await _lockRepo.UnLockAsync(id) == false)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Request UnLock Failed");
            return;
        }
    }
}