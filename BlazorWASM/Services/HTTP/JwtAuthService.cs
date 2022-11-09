using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Shared.DTOs;
using Shared.Models;

namespace BlazorWASM.Services.HTTP;

public class JwtAuthService : IAuthService
{
    private readonly HttpClient client;

    public JwtAuthService(HttpClient client)
    {
        this.client = client;
    }

    // This is a  private variable used to store the JWT token!
    public static string? JWT { get; private set; } = "";

    public Task<ClaimsPrincipal> GetAuthAsync()
    {
        ClaimsPrincipal principal = CreateClaimsPrincipal();
        return Task.FromResult(principal);
    }

    public Action<ClaimsPrincipal> OnAuthStateChanged { get; set; } = null!;

    // Below methods stolen from https://github.com/SteveSandersonMS/presentation-2019-06-NDCOslo/blob/master/demos/MissionControl/MissionControl.Client/Util/ServiceExtensions.cs
    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        string payload = jwt.Split('.')[1];
        byte[] jsonBytes = ParseBase64WithoutPadding(payload);
        Dictionary<string, object>? keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2:
                base64 += "==";
                break;
            case 3:
                base64 += "=";
                break;
        }

        return Convert.FromBase64String(base64);
    }
    
    
    // JWT is "converted" into a ClaimsPrincipal object here
    
    private static ClaimsPrincipal CreateClaimsPrincipal()
    {
        if (string.IsNullOrEmpty(JWT))
        {
            return new ClaimsPrincipal();
        }

        IEnumerable<Claim> claims = ParseClaimsFromJwt(JWT);
    
        ClaimsIdentity identity = new(claims, "jwt");

        ClaimsPrincipal principal = new(identity);
        return principal;
    }

    public async Task LoginAsync(string username, string password)
    {
        UserLoginDTO userLoginDto = new()
        {
            Username = username,
            Password = password
        };

        string userAsJson = JsonSerializer.Serialize(userLoginDto);
        StringContent content = new(userAsJson, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync("/auth/login", content);
        string responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(responseContent);
        }

        string token = responseContent;
        JWT = token;

        ClaimsPrincipal principal = CreateClaimsPrincipal();

        OnAuthStateChanged.Invoke(principal);
    }

    public Task LogoutAsync()
    {
        JWT = null;
        ClaimsPrincipal principal = new();
        OnAuthStateChanged.Invoke(principal);
        return Task.CompletedTask;
    }

    public async Task RegisterAsync(User user)
    {
        string userAsJson = JsonSerializer.Serialize(user);
        StringContent content = new(userAsJson, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync("https://localhost:7130/auth/register", content);
        string responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(responseContent);
        }
    }
}