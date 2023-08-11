using MystiickWeb.Shared.Configs;
using MystiickWeb.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

MystiickWeb.Core.Startup.Init();
MystiickWeb.Clients.Startup.Init();

builder.Services
    .AddInjectables()
    .Configure<ConnectionStrings>(builder.Configuration.GetSection(ConnectionStrings.ConnectionStringsKey));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
