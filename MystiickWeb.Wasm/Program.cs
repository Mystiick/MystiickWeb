using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MystiickWeb.Wasm;
using MystiickWeb.Shared.Services;
using Microsoft.AspNetCore.Components.Authorization;
using MystiickWeb.Wasm.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<CacheService>();

// Identity services
builder.Services.AddApiAuthorization();
builder.Services.AddOptions();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<MystiickAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(services => services.GetRequiredService<MystiickAuthStateProvider>());
await builder.Build().RunAsync();
