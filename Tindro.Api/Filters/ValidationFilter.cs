using Microsoft.AspNetCore.Mvc.Filters;

public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var arg in context.ActionArguments.Values)
        {
            if (arg == null) continue;

            var props = arg.GetType().GetProperties();

            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(string))
                {
                    var value = prop.GetValue(arg) as string;
                    if (!string.IsNullOrWhiteSpace(value) &&
                        value.Contains("<") || value.Contains(">"))
                    {
                        throw new Exception("Invalid input detected");
                    }
                }
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
