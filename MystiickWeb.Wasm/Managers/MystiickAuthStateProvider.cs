using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

using MystiickWeb.Shared.Constants;
using MystiickWeb.Shared.Models.User;

using Newtonsoft.Json;

using System.Security.Claims;

namespace MystiickWeb.Wasm.Auth;

public class MystiickAuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _jsruntime;
    public MystiickAuthStateProvider(HttpClient http, IJSRuntime jsruntime)
    {
        _http = http;
        _jsruntime = jsruntime;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Get antiforgery token, and add it to the request
        var token = await _jsruntime.InvokeAsync<string>("Cookie.get", CookieConstants.AntiForgeryToken);

        // Remove the Antiforgery token if it already exists to put a clean one in
        if (_http.DefaultRequestHeaders.Contains("X-CSRF-TOKEN"))
            _http.DefaultRequestHeaders.Remove("X-CSRF-TOKEN");

        // Add Antiforgery token
        _http.DefaultRequestHeaders.Add("X-CSRF-TOKEN", token);
        User deserializedUser = JsonConvert.DeserializeObject<User>(await _http.GetStringAsync($"{ControllerConstants.Users}/current"));
        ClaimsIdentity webUser = new();

        if (deserializedUser.Authenticated)
        {
            webUser = new ClaimsIdentity(deserializedUser.Claims.Select(x => new Claim(x.ClaimType, x.ClaimValue)), "cookie");
        }

        return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(webUser)));
    }

    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
