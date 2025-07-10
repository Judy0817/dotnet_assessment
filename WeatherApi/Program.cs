using WeatherApi.Services;
using DotNetEnv;

// Load environment variables from .env file
Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", ".env"));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IOpenWeatherService, OpenWeatherService>();

builder.Services.AddScoped<IExecuteDatabaseQueries, ExecuteDatabaseQueries>();
builder.Services.AddScoped<IOpenWeatherService, OpenWeatherService>();
builder.Services.AddScoped<IDatabaseInitializationService, DatabaseInitializationService>();

//GenAI
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var initService = scope.ServiceProvider.GetRequiredService<IDatabaseInitializationService>();
    await initService.InitializeDatabase();
}

app.Run();
