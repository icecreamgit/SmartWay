using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartWay.EFCore;
using SmartWay.Postgres;
using SmartWay.Postgres.Interfaces;
using SmartWay.Postgres.Models;
using SmartWay.Services;
using System.ComponentModel.DataAnnotations;


ILoggerEasy logger = new LoggerEasy();
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<SmartWay.Postgres.DbContext>(xx => new SmartWay.Postgres.DbContext(connectionString, logger));
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString, b => b.MigrationsAssembly("SmartWay.EFCore")));


builder.Services.AddScoped<StaffEFCoreService>();
builder.Services.AddScoped<BlogPostService>();
builder.Services.AddSingleton<ILoggerEasy, LoggerEasy>();

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
