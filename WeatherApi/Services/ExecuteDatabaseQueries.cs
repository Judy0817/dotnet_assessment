using System.Data.SqlClient;
using WeatherApi.Models;

namespace WeatherApi.Services
{
    public interface IExecuteDatabaseQueries
    {
        Task<Weather?> GetWeatherByCoordinatesAsync(decimal latitude, decimal longitude);
        Task<List<Weather>> GetAllWeatherAsync();
        Task<Weather?> GetWeatherByIdAsync(int id);
        Task<Weather> SaveWeatherAsync(Weather weather);
    }

    public class ExecuteDatabaseQueries : IExecuteDatabaseQueries
    {
        private readonly string _connectionString;

        public ExecuteDatabaseQueries(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<Weather?> GetWeatherByCoordinatesAsync(decimal latitude, decimal longitude)
        {
            const string query = @"
                SELECT Id, Latitude, Longitude, Temperature, WeatherMain, WeatherDescription
                FROM Weather 
                WHERE ABS(Latitude - @Latitude) < 0.01 AND ABS(Longitude - @Longitude) < 0.01";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Latitude", latitude);
            command.Parameters.AddWithValue("@Longitude", longitude);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapReaderToWeather(reader);
            }

            return null;
        }

        public async Task<List<Weather>> GetAllWeatherAsync()
        {
            const string query = @"
                SELECT Id, Latitude, Longitude, Temperature, WeatherMain, WeatherDescription
                FROM Weather 
                ORDER BY Id DESC";

            var weatherList = new List<Weather>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                weatherList.Add(MapReaderToWeather(reader));
            }

            return weatherList;
        }

        public async Task<Weather?> GetWeatherByIdAsync(int id)
        {
            const string query = @"
                SELECT Id, Latitude, Longitude, Temperature, WeatherMain, WeatherDescription
                FROM Weather 
                WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapReaderToWeather(reader);
            }

            return null;
        }

        public async Task<Weather> SaveWeatherAsync(Weather weather)
        {
            // GenAI
            const string upsertQuery = @"
                MERGE Weather AS target
                USING (SELECT @Latitude AS Latitude, @Longitude AS Longitude, @Temperature AS Temperature,
                              @WeatherMain AS WeatherMain, @WeatherDescription AS WeatherDescription) AS source
                ON (ABS(target.Latitude - source.Latitude) < 0.01 AND ABS(target.Longitude - source.Longitude) < 0.01)
                WHEN MATCHED THEN
                    UPDATE SET Temperature = source.Temperature, WeatherMain = source.WeatherMain,
                               WeatherDescription = source.WeatherDescription
                WHEN NOT MATCHED THEN
                    INSERT (Latitude, Longitude, Temperature, WeatherMain, WeatherDescription)
                    VALUES (source.Latitude, source.Longitude, source.Temperature, source.WeatherMain,
                            source.WeatherDescription)
                OUTPUT INSERTED.Id, INSERTED.Latitude, INSERTED.Longitude, INSERTED.Temperature,
                       INSERTED.WeatherMain, INSERTED.WeatherDescription;";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(upsertQuery, connection);
            
            command.Parameters.AddWithValue("@Latitude", weather.Latitude);
            command.Parameters.AddWithValue("@Longitude", weather.Longitude);
            command.Parameters.AddWithValue("@Temperature", weather.Temperature);
            command.Parameters.AddWithValue("@WeatherMain", weather.WeatherMain);
            command.Parameters.AddWithValue("@WeatherDescription", weather.WeatherDescription);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapReaderToWeather(reader);
            }

            throw new InvalidOperationException("Failed to save weather data.");
        }

        private Weather MapReaderToWeather(SqlDataReader reader)
        {
            return new Weather
            {
                Id = reader.GetInt32(0),
                Latitude = reader.GetDecimal(1),
                Longitude = reader.GetDecimal(2),
                Temperature = reader.GetDecimal(3),
                WeatherMain = reader.GetString(4),
                WeatherDescription = reader.GetString(5)
            };
        }
    }
}
