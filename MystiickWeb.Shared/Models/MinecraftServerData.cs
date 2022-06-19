namespace MystiickWeb.Shared.Models;

public class MinecraftServerData
{
    public int CountOnline { get; set; }
    public int CountMaximum { get; set; }
    public string[] OnlinePlayerNames { get; set; } = Array.Empty<string>();
    public MinecraftPlayer[] OnlinePlayers { get; set; } = Array.Empty<MinecraftPlayer>();

    public int Day { get; set; }
    public int Time { get; set; }
    public string Difficulty { get; set; } = string.Empty;
    public string ServerName { get; set; } = string.Empty;
}
