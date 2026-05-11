using FinalProjectManager.Data.Constants;

namespace FinalProjectManager.Web.Middleware;

public class PendingApprovalMiddleware
{
    private readonly RequestDelegate _next;

    public PendingApprovalMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var user = context.User;
        var path = context.Request.Path;

        if (user.Identity?.IsAuthenticated == true
            && user.IsInRole(AppRoles.Supervisor)
            && user.FindFirst("IsApproved")?.Value == "false"
            && !path.StartsWithSegments("/SupervisorRegistration/PendingApproval")
            && !path.StartsWithSegments("/Identity/Account/Logout"))
        {
            context.Response.Redirect("/SupervisorRegistration/PendingApproval");
            return;
        }

        await _next(context);
    }
}
