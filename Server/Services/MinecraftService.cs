using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using RconSharp;

using MystiickWeb.Shared.Models;

namespace MystiickWeb.Server.Services;

public class MinecraftService : IDisposable
{
    private readonly ILogger<MinecraftService> _logger;
    private readonly RconClient _client;
    private readonly IConfiguration _config;

    public MinecraftService(ILogger<MinecraftService> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
        _client = RconClient.Create(_config[Secrets.MINECRAFT_RCON_IP], int.Parse(_config[Secrets.MINECRAFT_RCON_PORT]));
    }

    public async Task<MinecraftServerData> GetServerData()
    {
        await _client.ConnectAsync();
        
        bool authenticated = await _client.AuthenticateAsync(_config[Secrets.MINECRAFT_RCON_PASSWORD]);
        
        if (authenticated)
        {
            var list = await _client.ExecuteCommandAsync("list");
            var day = await _client.ExecuteCommandAsync("time query day");
            var time = await _client.ExecuteCommandAsync("time query daytime");
            var difficulty = await _client.ExecuteCommandAsync("difficulty");

            return ParseData(list, day, time, difficulty);
        }
        else
        {
            throw new UnauthorizedAccessException("Unable to authenticate to Minecraft RCON");
        }
    }

    /// <summary></summary>
    /// <param ref="list">String resembeling: // There are # of a max of # players online: {0}, {1}, {2}</param>
    private MinecraftServerData ParseData(string list, string day, string time, string difficulty)
    {
        var output = new MinecraftServerData();
        var words = list.Split(' ');

        output.CountOnline = int.Parse(words[2]);
        output.CountMaximum = int.Parse(words[7]);

        if (output.CountOnline > 0)
        {
            output.OnlinePlayers = words.Skip(10).Select(x => x.Replace(",", "")).ToArray();
        }
        else 
        {
            output.OnlinePlayers = new string[0];
        }

        output.Day = int.Parse(day.Substring("The time is ".Length));
        output.Time = int.Parse(time.Substring("The time is ".Length)) / 1000 + 6;
        output.Difficulty = difficulty.Substring("The difficulty is ".Length);

        return output;
    }

#region " IDisposable Implementation "
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
