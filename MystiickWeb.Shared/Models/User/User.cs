using System.Security.Claims;

namespace MystiickWeb.Shared.Models.User;

public class User
{
    public Guid ID { get; init; }
    public string Username { get; set; } = string.Empty;
    public string NormalizedUsername { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public List<UserClaim> Claims { get; init; }
    public bool Authenticated { get; init; }

    public bool AccountLocked { get => LockoutEndDate > DateTime.UtcNow; }
    public int FailedAttempts { get; set; }
    public DateTimeOffset LockoutEndDate { get; set; }

    public User()
    {
        Claims = new List<UserClaim>();
    }

    public User(ClaimsPrincipal user)
    {
        string id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        if (string.IsNullOrWhiteSpace(id))
        {
            Authenticated = false;
            ID = Guid.Empty;
            Claims = new();
        }
        else
        {
            Authenticated = true;
            ID = Guid.Parse(id);
            Username = user.FindFirst(ClaimTypes.Name)?.Value ?? "";
            Claims = user.Claims.Select(x => new UserClaim(x)).ToList();
        }
    }
}
