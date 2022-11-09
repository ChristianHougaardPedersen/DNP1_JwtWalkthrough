using Shared.Models;

namespace WebAPI.Services;

public interface IAuthService
{
    Task<User> ValidateUser(string username, string password);
    
    // RegisterUser not used in this tutorial
    Task RegisterUser(User user);
}