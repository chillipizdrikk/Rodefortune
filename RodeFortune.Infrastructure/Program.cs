using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using RodeFortune.Infrastructure.Configuration;
using RodeFortune.Infrastructure.Data;
DotNetEnv.Env.Load();
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
var builder = WebApplication.CreateBuilder(args);

// Додаємо MongoDB в DI-контейнер
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<MongoDbContext>();

var app = builder.Build();
app.Run();
