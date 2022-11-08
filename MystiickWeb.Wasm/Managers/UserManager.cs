using Microsoft.JSInterop;

using MystiickWeb.Shared;
using MystiickWeb.Shared.Constants;
using MystiickWeb.Shared.Models;
using MystiickWeb.Shared.Models.User;
using MystiickWeb.Wasm.Auth;

namespace MystiickWeb.Wasm.Managers;

[Injectable(typeof(UserManager))]
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

        if (output.Success)
            _authState.NotifyAuthenticationStateChanged();

        return output;
    }

    public async Task<Response> Logout()
    {
        var output = await PostApiAsync($"{ControllerConstants.Users}/logout");

        if (output.Success)
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

    public async Task<Response> ChangePassword(Credential credential, Credential newPassword)
    {
        await Task.CompletedTask;
        return null;
    }

    public async Task<Response> ChangeUsername(Credential credential, string newUsername)
    {
        var output = await PutApiAsync($"{ControllerConstants.Users}/current?username={newUsername}", credential);

        if (output.Success)
            _authState.NotifyAuthenticationStateChanged();

        return output;
    }
}
