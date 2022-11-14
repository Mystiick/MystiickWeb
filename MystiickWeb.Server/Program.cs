using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;

using MystiickWeb.Clients.Identity;
using MystiickWeb.Shared.Configs;
using MystiickWeb.Shared.Constants;
using MystiickWeb.Shared.Extensions;
using MystiickWeb.Shared.Models.User;

var builder = WebApplication.CreateBuilder(args);

MystiickWeb.Core.Startup.Init();

builder.Services
    .AddInjectables()
    .Configure<ConnectionStrings>(builder.Configuration.GetSection(ConnectionStrings.ConnectionStringsKey))
    .Configure<IdentityConfig>(builder.Configuration.GetSection(IdentityConfig.IdentityConfigsKey))
    .Configure<Features>(builder.Configuration.GetSection(Features.FeaturesKey));

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Identity and services
builder.Services.AddIdentityCore<User>();
builder.Services
    .AddScoped<IUserStore<User>, MystiickUserStore>()
    .AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN")
    .AddHttpContextAccessor()
    .AddAuthentication("cookies")
    .AddCookie("cookies", x => x.LoginPath = "/user/login");

// Configs
builder.Configuration
    .AddJsonFile("appsettings.json", false)
    .AddJsonFile("appsettings.development.json", true)
    .AddEnvironmentVariables();

builder.Logging.AddConfiguration(builder.Configuration);
builder.Services.AddLogging(x =>
{
    x.AddFile(builder.Configuration.GetSection("Logging"));
});

#if DEBUG
builder.Logging.AddSimpleConsole(config => config.SingleLine = true);
#endif

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    // Required since this will be sitting behind a reverse proxy
    app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });

    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseAuthentication();

// Setup CSRF Token
app.Use((context, next) =>
{
    IAntiforgery? antiforgery = app.Services.GetRequiredService<IAntiforgery>();
    AntiforgeryTokenSet tokens = antiforgery.GetAndStoreTokens(context);
    context.Response.Cookies.Append(CookieConstants.AntiForgeryToken, tokens.RequestToken ?? "", new CookieOptions() { HttpOnly = false, IsEssential = true });

    return next(context);
});

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();


FileExtensionContentTypeProvider content = new();
content.Mappings[".pck"] = "application/octet-stream";

app.UseStaticFiles(new StaticFileOptions()
{
    ContentTypeProvider = content,
    OnPrepareResponse = ctx =>
    {
        app.Logger.LogInformation("Static file: {PhysicalPath}", ctx.File.PhysicalPath);
    }
});

app.UseRouting();
app.UseAuthorization();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
