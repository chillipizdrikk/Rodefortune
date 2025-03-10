using RodeFortune.DAL;
using RodeFortune.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Налаштовуємо MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));
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

var app = builder.Build();
app.Run();
