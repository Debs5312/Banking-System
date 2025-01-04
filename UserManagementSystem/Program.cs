using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistance;
using UserManagementSystem.Service;
using UserManagementSystem.Service.IService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

Connection connection = new Connection();
builder.Services.AddDbContext<AppDBContext>(x => x.UseSqlServer(connection.ConnectionString));

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"]));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt => 
{
  opt.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = key,
    ValidateIssuer = false,
    ValidateAudience = false
  };

});

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IUserService, UserService>();


var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseRouting();
app.UseCors(opt =>
{
    opt.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:3000");
});

app.UseAuthentication();
//app.UseAuthorization();

app.MapControllers();

app.Run();


