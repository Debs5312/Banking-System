using System.Text;
using System.Text.Json.Serialization;
using AccountManagementSystem.Services;
using AccountManagementSystem.Services.IServices;
using AccountManagementSystem.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistance;

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
// This is the standard and recommended way to handle configuration.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDBContext>(options => options.UseSqlServer(connectionString));

// Configure JWT Authentication based on the package reference in the .csproj
var tokenKeyString = builder.Configuration["TokenKey"];
if (string.IsNullOrEmpty(tokenKeyString) || Encoding.UTF8.GetBytes(tokenKeyString).Length < 32)
{
    // A missing or insecure token key is a critical configuration error.
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
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// Configure CORS policy to align with UserManagementSystem
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

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

var app = builder.Build();

// --- HTTP Request Pipeline Configuration ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
