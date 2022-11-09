using System.ComponentModel.DataAnnotations;
using Shared.Models;

namespace WebAPI.Services;

// TODO Remember to add this to Program.cs -> builder.Services.AddScoped

public class AuthService : IAuthService
{
    
    // This is just a "lazy" database with 2 users hardcoded!
    private readonly IList<User> users = new List<User>
    {
        new User
        {
            Age = 36,
            Email = "trmo@via.dk",
            Domain = "via",
            Name = "Troels Mortensen",
            Password = "onetwo3FOUR",
            Role = "Teacher",
            Username = "trmo",
            SecurityLevel = 4
        },
        new User
        {
            Age = 34,
            Email = "jakob@gmail.com",
            Domain = "gmail",
            Name = "Jakob Rasmussen",
            Password = "password",
            Role = "Student",
            Username = "jknr",
            SecurityLevel = 2
        }
    };

    public Task<User> ValidateUser(string username, string password)
    {
        User? existingUser = users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        
        // If user does not exists - will be handled in Controllers
        if (existingUser == null)
        {
            throw new Exception("User not found");
        }
        
        // If password is wrong
        if (!existingUser.Password.Equals(password))
        {
            throw new Exception("Password mismatch");
        }
        
        
        return Task.FromResult(existingUser);

    }
    
    public Task RegisterUser(User user)
    {

        if (string.IsNullOrEmpty(user.Username))
        {
            throw new ValidationException("Username cannot be null");
        }

        if (string.IsNullOrEmpty(user.Password))
        {
            throw new ValidationException("Password cannot be null");
        }
        // Do more user info validation here
        
        // save to persistence instead of list
        
        users.Add(user);
        
        return Task.CompletedTask;
    }
    
    

}