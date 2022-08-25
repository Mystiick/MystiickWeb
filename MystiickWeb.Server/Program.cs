using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;

using MystiickWeb.Core.Interfaces.Clients;
using MystiickWeb.Core.Interfaces.Services;
using MystiickWeb.Core.Services;
using MystiickWeb.Clients;
using MystiickWeb.Clients.Identity;
using MystiickWeb.Clients.Images;
using MystiickWeb.Shared.Configs;
using MystiickWeb.Shared.Models.User;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Services
builder.Services.AddScoped<IMinecraftService, MinecraftService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IPostService, PostService>();

// Clients
builder.Services.AddScoped<IImageDataClient, ImageDataClient>();
builder.Services.AddScoped<IImageFileClient, ImageFileClient>();
builder.Services.AddScoped<IPostDataClient, PostDataClient>();

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
