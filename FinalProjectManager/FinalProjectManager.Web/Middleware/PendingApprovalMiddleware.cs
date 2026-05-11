using FinalProjectManager.Data.Constants;

namespace FinalProjectManager.Web.Middleware;

public class PendingApprovalMiddleware
{
    private readonly RequestDelegate _next;

    private static readonly PathString[] _allowedPaths =
    [
        new("/SupervisorRegistration/PendingApproval"),
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

        var isUnapprovedSupervisor =
            user.Identity?.IsAuthenticated == true
            && user.IsInRole(AppRoles.Supervisor)
            && user.FindFirst("IsApproved")?.Value == "false";

        if (isUnapprovedSupervisor && !IsAllowed(path))
        {
            context.Response.Redirect("/SupervisorRegistration/PendingApproval");
            return;
        }

        await _next(context);
    }

    private static bool IsAllowed(PathString path) =>
        Array.Exists(_allowedPaths, allowed =>
            path.StartsWithSegments(allowed, StringComparison.OrdinalIgnoreCase));
}
