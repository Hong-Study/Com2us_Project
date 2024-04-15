public class ResponseCheckMiddleware
{
    public RequestDelegate _next = null!;

    public ResponseCheckMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);
    }
}