using MystiickWeb.Shared.Models;

namespace MystiickWeb.Core.Interfaces.Services;

public interface IMinecraftService
{
    Task<MinecraftServerData> GetServerData();
    Task<MinecraftPlayer> GetPlayerData(string name);
}
