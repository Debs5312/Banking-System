using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistance;
using UserManagementSystem.Service;
using UserManagementSystem.Service.IService;

const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// --- Service Configuration ---

// Using ReferenceHandler.Preserve can be a sign of object cycles in your entities.
// While it fixes serialization, it produces non-standard JSON ($id, $ref) which can be hard for clients.
// The recommended long-term solution is to use Data Transfer Objects (DTOs) to shape your API responses and break cycles.
builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

// Add services for API documentation (Swagger/OpenAPI)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext to use the connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDBContext>(options => options.UseSqlServer(connectionString));

// Configure JWT Authentication
var tokenKeyString = builder.Configuration["TokenKey"];
if (string.IsNullOrEmpty(tokenKeyString) || Encoding.UTF8.GetBytes(tokenKeyString).Length < 32)
{
    // This check is duplicated from TokenService but is critical to ensure the app doesn't start
    // with an insecure key for token *validation*. A better long-term solution is the Options Pattern.
    throw new InvalidOperationException("TokenKey is not configured or is not long enough for HMAC-SHA256 security.");
}
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKeyString));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = false, // In production, you might want to validate the issuer
            ValidateAudience = false // In production, you might want to validate the audience
        };
    });

// Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            // In production, restrict this to your actual frontend domain
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// --- HTTP Request Pipeline Configuration ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization(); // This was commented out, it's required for [Authorize] attributes to work.

app.MapControllers();

app.Run();
