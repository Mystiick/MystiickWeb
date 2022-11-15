using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using MystiickWeb.Shared.Configs;

namespace MystiickWeb.Server.Controllers
{

    [ApiController]
    [Route(Shared.Constants.ControllerConstants.Features)]
    public class FeaturesController
    {

        private Features _features;

        public FeaturesController(IOptions<Features> features)
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
}
