//TODO Remember to add Microsoft.AspNetCore.Authentication.JwtBearer Nuget! (latest version does not work with .Net 6)

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shared.Auth;
using WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// The builder.Condiguration["Jwt:Stuff"] means that we are retrieving information from the appdata.json file.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

//This is our class from the Shared project, so here we tell the Web API to add authorization policies,
// which will be used when checking if a client can call an endpoint.

AuthorizationPolicies.AddPolicies(builder.Services);

builder.Services.AddScoped<IAuthService, AuthService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// This adds authentication middleware - if caller for a specific endpoint is not authenticated it will return 401 - Unauthorized.
// NOTE: This must be above app.UseAuthorization
app.UseAuthentication();

app.UseAuthorization();

// Lets app accept requests from browser.
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allows any origin
    .AllowCredentials());

app.MapControllers();

app.Run();