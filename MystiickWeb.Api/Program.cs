using MystiickWeb.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

MystiickWeb.Core.Startup.Init();
MystiickWeb.Clients.Startup.Init();

builder.Services.AddInjectables();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
