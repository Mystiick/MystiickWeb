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

    protected async override Task<Response> PostApiAsync(string path, object? request = null)
    {
        var output = await base.PostApiAsync(path, request);

        if (output.Success)
            _authState.NotifyAuthenticationStateChanged();

        return output;
    }

    public async Task<Response> Login(Credential credential) => await PostApiAsync($"{ControllerConstants.Users}/login", credential);
    public async Task<Response> Logout() => await PostApiAsync($"{ControllerConstants.Users}/logout");
    public async Task<Response> RegisterUser(Credential credential) => await PostApiAsync($"{ControllerConstants.Users}/register", credential);
    public async Task<Response> ChangePassword(Credential credential, Credential newPassword) => await PutApiAsync($"{ControllerConstants.Users}/current/password", new[] { credential, newPassword });
    public async Task<Response> ChangeUsername(Credential credential, string newUsername) => await PutApiAsync($"{ControllerConstants.Users}/current?username={newUsername}", credential);
    public async Task<Response> AddRoleToUser(string userID, string role) => await PostApiAsync($"{ControllerConstants.Users}/{userID}", role);
    public async Task<Response<User>> LookupUser(string username) => await GetFromApiAsync<User>($"{ControllerConstants.Users}?username={username}");

}
