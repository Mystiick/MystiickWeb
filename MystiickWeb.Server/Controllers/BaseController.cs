using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MystiickWeb.Server.Controllers;

public class BaseController : Controller
{
    private readonly ILogger _logger;

    public BaseController(ILogger logger)
    {
        this._logger = logger;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        StringBuilder sb = new("[");
        sb.Append(filterContext.HttpContext.Connection.RemoteIpAddress);
        sb.Append("] ");
        sb.Append(filterContext.HttpContext.Request.Method);
        sb.Append(' ');
        sb.Append(filterContext.HttpContext.Request.Scheme);
        sb.Append(' ');
        sb.Append(filterContext.HttpContext.Request.Host.Value);
        sb.Append(' ');
        sb.Append(filterContext.HttpContext.Request.Path);
        sb.Append(' ');
        sb.Append(filterContext.HttpContext.Request.QueryString.Value);

        _logger.LogInformation("{log}", sb.ToString());
    }
}
