using WeatherApi.Models;
using System.Data.SqlClient;
using System.Net;

namespace WeatherApi.Services
{
    public interface IDatabaseInitializationService
    {
        Task InitializeDatabase();
    }

    public class DatabaseInitializationService : IDatabaseInitializationService
    {
        private readonly IOpenWeatherService _openWeatherService;
        private readonly IExecuteDatabaseQueries _executeDatabaseQueries;
        private readonly ILogger<DatabaseInitializationService> _logger;

        // GenAI
        private readonly (decimal Lat, decimal Lon)[] _randomCoordinates = new (decimal, decimal)[]
        {
            (51.5074m, -0.1278m),    // London, UK
            (40.7128m, -74.0060m),   // New York, USA
            (35.6762m, 139.6503m),   // Tokyo, Japan
            (-33.8688m, 151.2093m),  // Sydney, Australia
            (48.8566m, 2.3522m),     // Paris, France
            (55.7558m, 37.6173m),    // Moscow, Russia
            (19.4326m, -99.1332m),   // Mexico City, Mexico
            (1.3521m, 103.8198m),    // Singapore
            (-23.5505m, -46.6333m),  // São Paulo, Brazil
            (30.0444m, 31.2357m)     // Cairo, Egypt
        };

        public DatabaseInitializationService(
            IOpenWeatherService openWeatherService,
            IExecuteDatabaseQueries executeDatabaseQueries,
            ILogger<DatabaseInitializationService> logger)
        {
            _openWeatherService = openWeatherService ?? throw new ArgumentNullException(nameof(openWeatherService));
            _executeDatabaseQueries = executeDatabaseQueries ?? throw new ArgumentNullException(nameof(executeDatabaseQueries));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InitializeDatabase()
        {
            try
            {
                var existingData = await _executeDatabaseQueries.GetAllWeatherAsync();
                if (existingData.Count >= _randomCoordinates.Length)
                {
                    return;
                }

                var successCount = 0;
                var errorCount = 0;

                foreach (var (lat, lon) in _randomCoordinates)
                {
                    try
                    {
                        var weather = await _openWeatherService.GetWeatherByCoordinatesAsync(lat, lon);
                        if (weather != null)
                        {
                            await _executeDatabaseQueries.SaveWeatherAsync(weather);
                            successCount++;
                        }
                        else
                        {
                            errorCount++;
                        }

                        await Task.Delay(500);
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during database initialization");
            }
        }
    }
}
