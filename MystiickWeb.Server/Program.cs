using MystiickWeb.Server.Services;
using MystiickWeb.Server.Clients;
using MystiickWeb.Server.Clients.Images;
using MystiickWeb.Shared.Configs;


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

// Configs
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection(ConnectionStrings.ConnectionStringsKey));
builder.Configuration.AddJsonFile("appsettings.json", false);
builder.Configuration.AddJsonFile("appsettings.development.json", true);
builder.Configuration.AddEnvironmentVariables();

builder.Logging.AddConfiguration(builder.Configuration);

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
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts(); 
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
