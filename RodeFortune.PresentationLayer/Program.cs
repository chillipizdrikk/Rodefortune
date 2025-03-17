using RodeFortune.DAL;
using RodeFortune.DAL.Repositories;
using DotNetEnv;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RodeFortune.BLL.Services.Implementations;

Env.Load();

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services));

    var connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");
    var databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME");

    builder.Services.Configure<MongoDbSettings>(options =>
    {
        options.ConnectionString = connectionString;
        options.DatabaseName = databaseName;
    });

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

    builder.Services.AddControllersWithViews();

//������ ��� ²���������� VIEW � DivinationService
builder.Services.AddScoped<DivinationService>();

builder.Services.AddControllersWithViews();
var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();

    Log.CloseAndFlush();
