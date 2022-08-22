using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;

using MystiickWeb.Server.Services;
using MystiickWeb.Server.Clients;
using MystiickWeb.Server.Clients.Images;
using MystiickWeb.Shared.Configs;
using MystiickWeb.Shared.Models.User;
using Microsoft.AspNetCore.Identity;
using MystiickWeb.Server.Clients.Identity;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Services
builder.Services.AddSingleton<MinecraftService>();
builder.Services.AddSingleton<ImageService>();
builder.Services.AddSingleton<PostService>();

// Clients
builder.Services.AddSingleton<ImageDataClient>();
builder.Services.AddSingleton<ImageFileClient>();
builder.Services.AddSingleton<PostDataClient>();

// Identity
builder.Services.AddIdentityCore<User>();
builder.Services.AddScoped<IUserStore<User>, MystiickUserStore>();

// Configs
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection(ConnectionStrings.ConnectionStringsKey));
builder.Configuration.AddJsonFile("appsettings.json", false);
builder.Configuration.AddJsonFile("appsettings.development.json", true);
builder.Configuration.AddEnvironmentVariables();

builder.Logging.AddConfiguration(builder.Configuration);
builder.Services.AddLogging(x =>
{
    x.AddFile(builder.Configuration.GetSection("Logging"));
});

#if DEBUG
builder.Logging.AddSimpleConsole(config => config.SingleLine = true);
#endif

var app = builder.Build();

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

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();


FileExtensionContentTypeProvider content = new();
content.Mappings[".pck"] = "application/octet-stream";

app.UseStaticFiles(new StaticFileOptions() { 
    ContentTypeProvider = content,
    OnPrepareResponse = ctx =>
    {
        app.Logger.LogInformation("Static file: {PhysicalPath}", ctx.File.PhysicalPath);
    }
});

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
