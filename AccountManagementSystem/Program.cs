using System.Text.Json.Serialization;
using AccountManagementSystem.Services;
using AccountManagementSystem.Services.IServices;
using AccountManagementSystem.Utils;
using Microsoft.EntityFrameworkCore;
using Persistance;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

Connection connection = new Connection();
builder.Services.AddDbContext<AppDBContext>(x => x.UseSqlServer(connection.ConnectionString));

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

var app = builder.Build();

//app.UseHttpsRedirection();
// Configure the HTTP request pipeline.
app.UseRouting();


app.MapControllers();
app.Run();

