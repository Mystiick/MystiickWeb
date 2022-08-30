using Microsoft.JSInterop;

using MystiickWeb.Shared;
using MystiickWeb.Shared.Constants;
using MystiickWeb.Shared.Models;

using System.Net.Http.Json;

namespace MystiickWeb.Wasm.Managers;

// [Injectable(typeof(IMinecraftManager))]
internal class MinecraftManager : BaseManager, IMinecraftManager
{
    public MinecraftManager(HttpClient http, IJSRuntime js) : base(http, js) { }

    public async Task<Response<MinecraftServerData>> GetMinecraftServerData()
    {
        var output = await GetFromApiAsync<MinecraftServerData>(ControllerConstants.Minecraft);

        return output;
    }
}

[Injectable(typeof(IMinecraftManager))]
public class MinecraftMockManager : IMinecraftManager
{
    public async Task<Response<MinecraftServerData>> GetMinecraftServerData()
    {
        return await Task.FromResult(new Response<MinecraftServerData>()
        {
            Value = new MinecraftServerData() { 
                CountMaximum = 20,
                CountOnline = 6,
                Day = 42,
                Difficulty = "Normal",
                ServerName = "Mock Server",
                OnlinePlayers = new MinecraftPlayer[] {
                    new MinecraftPlayer() { Health = 10, Hunger = 10, Level = 99, Name = "kaisan"},
                    new MinecraftPlayer() { Health = 10, Hunger = 6, Level = 99, Name = "mystiick"},
                    new MinecraftPlayer() { Health = 2, Hunger = 8, Level = 99, Name = "player1"},
                    new MinecraftPlayer() { Health = 7, Hunger = 1, Level = 99, Name = "player2"},
                    new MinecraftPlayer() { Health = 10, Hunger = 10, Level = 99, Name = "player3"},
                    new MinecraftPlayer() { Health = 0, Hunger = 0, Level = 99, Name = "player4"}
                },
                Time = 10
            },
            Success = true
        });
    }
}