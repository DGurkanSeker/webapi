using backendProjesi.Models;
using Lucene.Net.Support;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webapi;
using webapi.Interfaces;
using WebApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>(); 

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddDbContext<UsersContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
