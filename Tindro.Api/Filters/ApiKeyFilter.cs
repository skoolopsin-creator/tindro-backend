using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ApiKeyFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("X-API-KEY", out var key))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (key != "SUPER_ADMIN_KEY")
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
