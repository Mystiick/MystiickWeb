using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;

using MystiickWeb.Wasm;
using MystiickWeb.Wasm.Auth;
using MystiickWeb.Shared.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddScoped<MystiickAuthStateProvider>()
    .AddScoped<AuthenticationStateProvider>(services => services.GetRequiredService<MystiickAuthStateProvider>())
    .AddInjectables();

// Identity services
builder.Services
    .AddOptions()
    .AddAuthorizationCore()
    .AddApiAuthorization();

await builder.Build().RunAsync();
