using Tindro.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Tindro.Infrastructure.Persistence;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddInfrastructure(builder.Configuration);

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


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    try
    {
        var cmd = scope.ServiceProvider.GetRequiredService<CommandDbContext>();
        var qry = scope.ServiceProvider.GetRequiredService<QueryDbContext>();

        cmd.Database.EnsureCreated();
        qry.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"DB init failed: {ex}");
        throw;
    }

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
