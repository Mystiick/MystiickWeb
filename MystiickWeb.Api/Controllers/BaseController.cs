using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MystiickWeb.Api.Controllers;

public abstract class BaseController : Controller
{
    protected readonly ILogger _logger;

    public BaseController(ILogger logger)
    {
        this._logger = logger;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        //_logger.LogInformation("{log}", BuildLog(filterContext.HttpContext));
    }

    protected virtual string BuildLog(HttpContext context, string message = "")
    {
        StringBuilder sb = new("[");
        sb.Append(context.Connection.RemoteIpAddress);
        sb.Append("] ");
        sb.Append(context.Request.Method);
        sb.Append(' ');
        sb.Append(context.Request.Scheme);
        sb.Append(' ');
        sb.Append(context.Request.Host.Value);
        sb.Append(' ');
        sb.Append(context.Request.Path);
        sb.Append(' ');
        sb.Append(context.Request.QueryString.Value);

        if (!string.IsNullOrWhiteSpace(message))
        {
            sb.Append(' ');
            sb.Append(message);
        }

        return sb.ToString();
    }
}
