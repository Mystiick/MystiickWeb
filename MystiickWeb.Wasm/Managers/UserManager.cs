using Microsoft.JSInterop;

using MystiickWeb.Shared.Constants;
using MystiickWeb.Shared.Models;
using MystiickWeb.Shared.Models.User;
using MystiickWeb.Wasm.Auth;

namespace MystiickWeb.Wasm.Managers;

internal class UserManager : BaseManager
{
    private MystiickAuthStateProvider _authState;

    public UserManager(HttpClient http, IJSRuntime js, MystiickAuthStateProvider masp) : base(http, js)
    {

        _authState = masp;

    }

    public async Task<Response> Login(Credential credential)
    {
        var output = await PostApiAsync($"{ControllerConstants.Users}/login", credential);
        _authState.NotifyAuthenticationStateChanged();

        return output;
    }

    public async Task<Response> Logout()
    {
        var output = await PostApiAsync($"{ControllerConstants.Users}/logout");
        _authState.NotifyAuthenticationStateChanged();

        return output;
    }

    public async Task<Response> RegisterUser(Credential credential)
    {
        var output = await PostApiAsync($"{ControllerConstants.Users}/register", credential);

        if (output.Success)
            output = await Login(credential);

        return output;
    }
}
