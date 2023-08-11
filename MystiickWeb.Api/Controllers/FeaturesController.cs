using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using MystiickWeb.Shared.Configs;

namespace MystiickWeb.Api.Controllers;


[ApiController]
[Route(Shared.Constants.ControllerConstants.Features)]
public class FeaturesController : BaseController
{
    private readonly Features _features;

    public FeaturesController(ILogger<FeaturesController> logger, IOptions<Features> features) : base(logger)
    {
        _features = features.Value;
    }

    [HttpGet]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public Features GetAllFeatures() => _features;

    [HttpGet("{feature}")]
    public bool IsFeatureEnabled(string feature) => _features.IsFeatureEnabled(feature);
}
