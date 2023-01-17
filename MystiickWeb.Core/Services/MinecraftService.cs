using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MystiickWeb.Core.Interfaces.Services;
using MystiickWeb.Shared;
using MystiickWeb.Shared.Constants;
using MystiickWeb.Shared.Models;
// TODO: Move most of this to a Client, and remove RconSharp dependency
using RconSharp;

namespace MystiickWeb.Core.Services;

[Injectable(typeof(IMinecraftService))]
public class MinecraftService : IMinecraftService, IDisposable
{
    private readonly ILogger<MinecraftService> _logger;
    private readonly RconClient _client;
    private readonly IConfiguration _config;

    public MinecraftService(ILogger<MinecraftService> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
        _client = RconClient.Create(_config[MinecraftSecrets.MINECRAFT_RCON_IP], int.Parse(_config[MinecraftSecrets.MINECRAFT_RCON_PORT]));
    }

    private async Task Connect()
    {
        await _client.ConnectAsync();
        bool authenticated = await _client.AuthenticateAsync(_config[MinecraftSecrets.MINECRAFT_RCON_PASSWORD]);

        if (!authenticated)
        {
            throw new UnauthorizedAccessException("Unable to authenticate to Minecraft RCON");
        }
    }

    public async Task<MinecraftServerData?> GetServerData()
    {
        try
        {
            await Connect();
        }
        catch
        {
            return null;
        }

        var list = await _client.ExecuteCommandAsync("list");
        var day = await _client.ExecuteCommandAsync("time query day");
        var time = await _client.ExecuteCommandAsync("time query daytime");
        var difficulty = await _client.ExecuteCommandAsync("difficulty");

        return ParseServerData(list, day, time, difficulty);
    }

    public async Task<MinecraftPlayer> GetPlayerData(string name)
    {
        await Connect();

        var level = await _client.ExecuteCommandAsync($"data get entity {name} XpLevel");
        var hunger = await _client.ExecuteCommandAsync($"data get entity {name} foodLevel");
        var health = (await _client.ExecuteCommandAsync($"data get entity {name} Health")).Replace("f", "");

        if (new[] {level, hunger, health}.Any(x => x == "No entity was found"))
        {
            throw new KeyNotFoundException(name);
        }

        return ParsePlayerData(name, level, hunger, health);
    }

    /// <summary></summary>
    /// <param ref="list">String resembeling: // There are # of a max of # players online: {0}, {1}, {2}</param>
    private MinecraftServerData ParseServerData(string list, string day, string time, string difficulty)
    {
        var output = new MinecraftServerData();
        var words = list.Split(' ');

        output.CountOnline = int.Parse(words[2]);
        output.CountMaximum = int.Parse(words[7]);

        if (output.CountOnline > 0)
        {
            output.OnlinePlayerNames = words.Skip(10).Select(x => x.Replace(",", "")).ToArray();
        }
        else 
        {
            output.OnlinePlayerNames = Array.Empty<string>();
        }

        output.Day = int.Parse(day.Substring("The time is ".Length));
        output.Time = int.Parse(time.Substring("The time is ".Length)) / 1000 + 6;
        output.Difficulty = difficulty.Substring("The difficulty is ".Length);

        return output;
    }

    private MinecraftPlayer ParsePlayerData(string name, string level, string hunger, string health)
    {
        var output = new MinecraftPlayer();
        
        output.Name = name;
        output.Level = int.Parse(level.Substring(level.IndexOf("data: ") + 6));
        output.Hunger = int.Parse(hunger.Substring(hunger.IndexOf("data: ") + 6)) / 2f;
        output.Health = float.Parse(health.Substring(health.IndexOf("data: ") + 6)) / 2f;

        return output;
    }

#region | IDisposable Implementation |
    private bool isDisposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (isDisposed) 
        {
            // Don't dispose multiple times
            return;
        }

        if (disposing)
        {
            // Cleanup resources here
            _client.Disconnect();
        }

        isDisposed = true;
    }
#endregion

}
