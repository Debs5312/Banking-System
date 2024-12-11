using AccountManagementSystem.Services;
using AccountManagementSystem.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Persistance;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

Connection connection = new Connection();
builder.Services.AddDbContext<AppDBContext>(x => x.UseSqlServer(connection.ConnectionString));

builder.Services.AddScoped<IAccountService, AccountService>();

var app = builder.Build();

//app.UseHttpsRedirection();
// Configure the HTTP request pipeline.
app.UseRouting();


app.MapControllers();
app.Run();

