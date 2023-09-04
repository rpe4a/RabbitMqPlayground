namespace MiddleWareInvestigation;

public class CountMiddleware
{
    private readonly RequestDelegate _next;

    private int count;

    public CountMiddleware(RequestDelegate next)
    {
        this.count = 0;
        this._next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        count++;
        await _next.Invoke(context);
    }
}