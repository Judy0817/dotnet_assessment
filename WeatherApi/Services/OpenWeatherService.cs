using WeatherApi.Models;
using System.Text.Json;

namespace WeatherApi.Services
{
    public interface IOpenWeatherService
    {
        Task<Weather?> GetWeatherByCoordinatesAsync(decimal latitude, decimal longitude);
    }

    public class OpenWeatherService : IOpenWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<OpenWeatherService> _logger;

        public OpenWeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<OpenWeatherService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY") 
                ?? throw new ArgumentException("OpenWeather API key not found in environment variables.");
        }

        public async Task<Weather?> GetWeatherByCoordinatesAsync(decimal latitude, decimal longitude)
        {
            try
            {                
                var url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={_apiKey}&units=metric";
                
                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("OpenWeather API returned status code: {StatusCode} for coordinates: {Latitude}, {Longitude}", 
                        response.StatusCode, latitude, longitude);
                    return null;
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                };
                
                var openWeatherResponse = JsonSerializer.Deserialize<OpenWeatherResponse>(jsonContent, options);

                if (openWeatherResponse == null)
                {
                    _logger.LogWarning("Failed to deserialize weather response for coordinates: {Latitude}, {Longitude}", latitude, longitude);
                    return null;
                }

                var weatherInfo = openWeatherResponse.Weather.FirstOrDefault();
                
                var weather = new Weather
                {
                    Latitude = openWeatherResponse.Coord.Lat,
                    Longitude = openWeatherResponse.Coord.Lon,
                    Temperature = openWeatherResponse.Main.Temp,
                    WeatherMain = weatherInfo?.Main ?? "Unknown",
                    WeatherDescription = weatherInfo?.Description ?? "Unknown"
                };

                return weather;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching weather data for coordinates: {Latitude}, {Longitude}", latitude, longitude);
                return null;
            }
        }
    }
}
