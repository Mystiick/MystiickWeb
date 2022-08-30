
using System.Security.Claims;

namespace MystiickWeb.Shared.Models.User;

public class UserClaim
{
    public string Issuer { get; set; } = string.Empty;
    public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string ValueType { get; set; } = string.Empty;

    public UserClaim(){}
    public UserClaim(Claim claim)
    {
        Issuer = claim.Issuer;
        Properties = claim.Properties;
        @Type = claim.Type;
        Value = claim.Value;
        ValueType = claim.ValueType;
    }
}
