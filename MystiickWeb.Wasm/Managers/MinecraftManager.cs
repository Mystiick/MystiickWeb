using Microsoft.JSInterop;

using MystiickWeb.Shared;
using MystiickWeb.Shared.Constants;
using MystiickWeb.Shared.Models;

using Newtonsoft.Json.Linq;

namespace MystiickWeb.Wasm.Managers;

[Injectable(typeof(MinecraftManager))]
internal class MinecraftManager : BaseManager
{
    public MinecraftManager(HttpClient http, IJSRuntime js) : base(http, js) { }

    public async Task<Response<MinecraftServerData>> GetMinecraftServerData(string serverName)
    {
        if (serverName == "mock")
        {
            await Task.Delay(1500);
            return new Response<MinecraftServerData>()
            {
                Value = new MinecraftServerData()
                {
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
            };
        }
        else
        {
            return await GetFromApiAsync<MinecraftServerData>(ControllerConstants.Minecraft);
        }
    }
}
