namespace Shared.Models;

// TODO Remember to add Microsoft.AspNetCore.Authorization NUGET to shared!

// Probably to many properties - used to show auth policies.
public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Domain { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    public int Age { get; set; }
    public int SecurityLevel { get; set; }
}