using System.Security.Claims;
using Shared.Models;

namespace BlazorWASM.Services;

public interface IAuthService
{
    // TODO Whole services directory should actually be in a seperate component! -> HttpClients Stuff??
    public Task LoginAsync(string username, string password);
    public Task LogoutAsync();
    public Task RegisterAsync(User user);
    public Task<ClaimsPrincipal> GetAuthAsync();

    public Action<ClaimsPrincipal> OnAuthStateChanged { get; set; }
}