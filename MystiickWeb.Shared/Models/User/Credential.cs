using System.ComponentModel.DataAnnotations;

namespace MystiickWeb.Shared.Models.User;

public class Credential
{
    public string Username { get; set; } = string.Empty;

    [DataType(DataType.Password)] 
    public string Password { get; set; } = string.Empty;

    [Compare(nameof(Password))]
    [DataType(DataType.Password)] 
    public string ConfirmPassword { get; set; } = string.Empty;
}
