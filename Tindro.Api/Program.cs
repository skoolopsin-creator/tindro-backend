using Tindro.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Tindro.Infrastructure.Persistence;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddInfrastructure(builder.Configuration);

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

Console.WriteLine("JWT KEY => " + jwtKey);
Console.WriteLine("JWT ISSUER => " + jwtIssuer);
Console.WriteLine("JWT AUDIENCE => " + jwtAudience);


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // 🔥 Disable Microsoft’s default claim remapping
      

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!)
            ),

            // 🔥 THIS TELLS ASP.NET: use "sub" as the user id
            NameClaimType = JwtRegisteredClaimNames.Sub,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var fcmProjectId = Environment.GetEnvironmentVariable("FCM_PROJECT_ID");
var fcmClientEmail = Environment.GetEnvironmentVariable("FCM_CLIENT_EMAIL");
var fcmPrivateKey = Environment.GetEnvironmentVariable("FCM_PRIVATE_KEY");

if (!string.IsNullOrWhiteSpace(fcmProjectId) &&
    !string.IsNullOrWhiteSpace(fcmClientEmail) &&
    !string.IsNullOrWhiteSpace(fcmPrivateKey))
{
    if (FirebaseApp.DefaultInstance == null)
    {
        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromJson($@"
        {{
          ""type"": ""service_account"",
          ""project_id"": ""{fcmProjectId}"",
          ""client_email"": ""{fcmClientEmail}"",
          ""private_key"": ""{fcmPrivateKey.Replace("\\n", "\n")}""
        }}")
        });
    }
}


var app = builder.Build();
var forwardOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto
};

// Render / Docker proxy fix
forwardOptions.KnownNetworks.Clear();
forwardOptions.KnownProxies.Clear();

app.UseForwardedHeaders(forwardOptions);


app.UseSwagger();
app.UseSwaggerUI();



using var scope = app.Services.CreateScope();

try
{
    var cmd = scope.ServiceProvider.GetRequiredService<CommandDbContext>();
    var qry = scope.ServiceProvider.GetRequiredService<QueryDbContext>();

    cmd.Database.Migrate();
    qry.Database.Migrate();
}
catch (Exception ex)
{
    Console.WriteLine($"DB init failed: {ex}");
    throw;
}



app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RateLimitMiddleware>();


if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<Tindro.Api.Hubs.ChatHub>("/hubs/chat");

app.Run();
