using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Auth;

// This is the class where we define the policies
public class AuthorizationPolicies
{
    
    // Method is going to be called from Program.cs classes in both WebAPI and Blazor -> will add auth policies to the framework.
    
    //Each line (options.AddPolicy) adds a new policy used to guard UI elements or WebApi endpoints.
    public static void AddPolicies(IServiceCollection services)
    {
        services.AddAuthorizationCore(options =>
        {
            // RequireAuthenticatedUser means that user has to be logged in and authenticated!
            
            
            // This means that a user must have a claim of type domain with value via.
            options.AddPolicy("MustBeVia", a =>
                a.RequireAuthenticatedUser().RequireClaim("Domain", "via"));

            // This means that a user must have a claim of type SecurityLevel with values 4 or 5.
            options.AddPolicy("SecurityLevel4", a =>
                a.RequireAuthenticatedUser().RequireClaim("SecurityLevel", "4", "5"));

            // This means that a user must have a claim of type Role with value teacher.
            options.AddPolicy("MustBeTeacher", a =>
                a.RequireAuthenticatedUser().RequireClaim("Role", "teacher"));

            // RequireAssertion means that if the logic inside of the statement returns true -> user fulfills this policy.
            // context contains a User property of type ClaimsPrincipal (object used by Authentication framework).
            // FindFirst finds the first claim of type SecurityLevel, and if it does not exist the user has no securitylevel (returns false)
            // Else - it checks that the securitylevel is at least 2.
            options.AddPolicy("SecurityLevel2OrAbove", a =>
                a.RequireAuthenticatedUser().RequireAssertion(context =>
                {
                    Claim? levelClaim = context.User.FindFirst(claim => claim.Type.Equals("SecurityLevel"));
                    if (levelClaim == null) return false;
                    return int.Parse(levelClaim.Value) >= 2;
                }));
        });
    }
    
    /*
     * In your project, either this tutorial, or other projects like SEP3, you will probably define some kind of User object,
     * maybe you call it User, Account, Profile, or something else. But it is a class you define to hold information about a user.
     *
     * This is your own custom object, and the Authentication functionality of Blazor and Web API
     * obviously does not know about your specific user type.
     *
     * Therefore we must convert our custom User into a class which the framework understands.
     * That class is the ClaimsPrincipal. How it's done will be covered later
     */

}