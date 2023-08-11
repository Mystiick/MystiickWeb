using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MystiickWeb.Shared.Configs;

namespace MystiickWeb.Api.Controllers;


[ApiController]
[Route(Shared.Constants.ControllerConstants.Features)]
public class FeaturesController : BaseController
{
    private Features _features;

    public FeaturesController(ILogger<FeaturesController> logger, IOptions<Features> features) : base(logger)
    {
        _features = features.Value;
    }

    [HttpGet]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public Features GetAllFeatures()
    {
        return _features;
    }

    [HttpGet("{feature}")]
    public bool IsFeatureEnabled(string feature)
    {
        return _features.IsFeatureEnabled(feature);
    }
}
