namespace OnlineStore.Middlewares;

public class SerilogUserEnricherMiddleware
{
    private readonly RequestDelegate _next;

    public SerilogUserEnricherMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var username = context.User?.Identity?.IsAuthenticated == true
            ? context.User.Identity.Name
            : "Anonymous";

        using (Serilog.Context.LogContext.PushProperty("UserName", username))
        {
            await _next(context);
        }
    }
}
