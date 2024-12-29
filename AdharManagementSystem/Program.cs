using System.Text.Json.Serialization;
using AdharManagementSystem.Services;
using AdharManagementSystem.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Persistance;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

Connection connection = new Connection();
builder.Services.AddDbContext<AppDBContext>(x => x.UseSqlServer(connection.ConnectionString));

builder.Services.AddScoped<IAdharService, AdharService>();
// builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

var app = builder.Build();


//app.UseHttpsRedirection();
// Configure the HTTP request pipeline.
app.UseRouting();
app.MapControllers();
app.Run();

