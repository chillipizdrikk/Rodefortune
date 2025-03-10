using RodeFortune.DAL;
using RodeFortune.DAL.Repositories;
using DotNetEnv;
using Serilog;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services));

var connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");
var databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME");

builder.Services.Configure<MongoDbSettings>(options => {
    options.ConnectionString = connectionString;
    options.DatabaseName = databaseName;
});

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddScoped<PostRepository>();
builder.Services.AddScoped<CommentRepository>();
builder.Services.AddScoped<ReadingRepository>();
builder.Services.AddScoped<HoroscopeRepository>();
builder.Services.AddScoped<TarotCardRepository>();
builder.Services.AddScoped<NatalChartRepository>();
builder.Services.AddScoped<HouseRepository>();
builder.Services.AddScoped<DestinyMatrixRepository>();

// Add MVC services
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

app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);
app.Run();
