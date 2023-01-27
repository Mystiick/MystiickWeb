namespace MystiickWeb.Shared.Configs;

public class IdentityConfig
{
    public const string IdentityConfigsKey = "IdentityConfig";
    public int MaxSignInAttempts { get; set; }
}
