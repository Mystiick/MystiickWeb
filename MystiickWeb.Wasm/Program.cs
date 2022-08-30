using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;

using MystiickWeb.Shared.Services;
using MystiickWeb.Wasm;
using MystiickWeb.Wasm.Auth;
using MystiickWeb.Wasm.Managers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddScoped<UserManager>()
    .AddScoped<MystiickAuthStateProvider>()
    .AddScoped<AuthenticationStateProvider>(services => services.GetRequiredService<MystiickAuthStateProvider>())
    .AddSingleton<CacheService>();

// Identity services
builder.Services
    .AddOptions()
    .AddAuthorizationCore()
    .AddApiAuthorization();

await builder.Build().RunAsync();
