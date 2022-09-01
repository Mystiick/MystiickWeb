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
        var log = $@"{filterContext.HttpContext.Request.Method} {filterContext.HttpContext.Request.Scheme} {filterContext.HttpContext.Request.Host.Value} {filterContext.HttpContext.Request.Path} {filterContext.HttpContext.Request.QueryString.Value}";

        _logger.LogInformation("{log}", log);
    }
}
