using MystiickWeb.Shared.Models;

namespace MystiickWeb.Wasm.Managers;

public interface IMinecraftManager
{
    Task<Response<MinecraftServerData>> GetMinecraftServerData();
}
