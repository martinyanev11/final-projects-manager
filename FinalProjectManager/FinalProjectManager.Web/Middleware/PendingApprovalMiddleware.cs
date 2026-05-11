using FinalProjectManager.Data.Constants;

namespace FinalProjectManager.Web.Middleware;

public class PendingApprovalMiddleware
{
    private readonly RequestDelegate _next;

    private static readonly PathString[] _allowedPaths =
    [
        new("/Account/PendingApproval"),
        new("/Identity"),
    ];

    public PendingApprovalMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var user = context.User;
        var path = context.Request.Path;

        var isUnapproved =
            user.Identity?.IsAuthenticated == true
            && user.FindFirst("IsApproved")?.Value == "false";

        if (isUnapproved && !IsAllowed(path))
        {
            context.Response.Redirect("/Account/PendingApproval");
            return;
        }

        await _next(context);
    }

    private static bool IsAllowed(PathString path) =>
        Array.Exists(_allowedPaths, allowed =>
            path.StartsWithSegments(allowed, StringComparison.OrdinalIgnoreCase));
}
