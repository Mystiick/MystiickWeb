
using System.Security.Claims;

namespace MystiickWeb.Shared.Models.User;

public class UserClaim
{
    public string ID { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    public string ClaimType { get; set; } = string.Empty;
    public string ClaimValue { get; set; } = string.Empty;
    public string ValueType { get; set; } = string.Empty;

    public UserClaim() { }
    public UserClaim(string type, string value)
    {
        ClaimType = type;
        ClaimValue = value;
    }
    public UserClaim(Claim claim)
    {
        Issuer = claim.Issuer;
        Properties = claim.Properties;
        ClaimType = claim.Type;
        ClaimValue = claim.Value;
        ValueType = claim.ValueType;
    }
}
