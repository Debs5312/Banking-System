using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Persistance;
using UserManagementSystem.Service;
using UserManagementSystem.Service.IService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

Connection connection = new Connection();
builder.Services.AddDbContext<AppDBContext>(x => x.UseSqlServer(connection.ConnectionString));

builder.Services.AddScoped<IUserService, UserService>();


var app = builder.Build();

//app.UseHttpsRedirection();
// Configure the HTTP request pipeline.
app.UseRouting();
app.MapControllers();
app.Run();


