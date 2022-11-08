using Microsoft.JSInterop;

using MystiickWeb.Shared;
using MystiickWeb.Shared.Configs;
using MystiickWeb.Shared.Constants;
using MystiickWeb.Shared.Models;

namespace MystiickWeb.Wasm.Managers;

[Injectable(typeof(FeaturesManager))]
internal class FeaturesManager : BaseManager
{
    public FeaturesManager(HttpClient http, IJSRuntime js) : base(http, js) { }

    public async Task<Response<Features>> GetFeatures()
    {
        return await GetFromApiAsync<Features>($"{ControllerConstants.Features}/");
    }
}
