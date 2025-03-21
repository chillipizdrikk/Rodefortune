using RodeFortune.DAL;
using RodeFortune.DAL.Repositories;
using DotNetEnv;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RodeFortune.BLL.Services.Implementations;
using RodeFortune.DAL.Repositories.Implementations;
using RodeFortune.DAL.Repositories.Interfaces;
using RodeFortune.BLL.Services.Interfaces;

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


builder.Services.AddScoped<ITarotCardRepository, TarotCardRepository>();
builder.Services.AddScoped<DivinationService>();
builder.Services.AddScoped<IHoroscopeRepository,HoroscopeRepository>();
builder.Services.AddScoped<IHoroscopeService, HoroscopeService>();
builder.Services.AddScoped<HoroscopeRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDestinyMatrixRepository, DestinyMatrixRepository>();
builder.Services.AddScoped<IReadingRepository, ReadingRepository>();
builder.Services.AddScoped<INatalChartRepository, NatalChartRepository>();
builder.Services.AddScoped<BloggingService>();



builder.Services.AddControllersWithViews();
var app = builder.Build();

//�������� �� ������ ����� try-catch, ���������� ������� � ������ ������� Error � HomeController
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
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
