namespace Shared.DTOs;

public class UserLoginDTO
{
    public string Username { get; init; }
    public string Password { get; init; } 
}

// Init = special kind of set.. Means that you can only set the values when creating the object - not later.