using RodeFortune.DAL;
using RodeFortune.DAL.Repositories;
using DotNetEnv;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

try
{
    // Load .env file
    Env.Load();

    var builder = WebApplication.CreateBuilder(args);

    // Налаштування Serilog з appsettings.json
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services));

    // Get MongoDB settings from .env file
    var connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");
    var databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME");

    // Configure MongoDB settings manually
    builder.Services.Configure<MongoDbSettings>(options =>
    {
        options.ConnectionString = connectionString;
        options.DatabaseName = databaseName;
    });

    // Register MongoDB client and database
    builder.Services.AddSingleton<IMongoClient>(sp =>
    {
        var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>();
        return new MongoClient(settings.Value.ConnectionString);
    });

    builder.Services.AddSingleton<IMongoDatabase>(sp =>
    {
        var client = sp.GetRequiredService<IMongoClient>();
        var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>();
        return client.GetDatabase(settings.Value.DatabaseName);
    });

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    // Додаємо Serilog request logging
    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Програма завершилась з помилкою");
}
finally
{
    // Додаємо обробку помилок при завершенні програми
    Log.CloseAndFlush();
}