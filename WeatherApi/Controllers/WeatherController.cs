using Microsoft.AspNetCore.Mvc;
using WeatherApi.Models;
using WeatherApi.Services;

namespace WeatherApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IOpenWeatherService _openWeatherService;
        private readonly IExecuteDatabaseQueries _executeDatabaseQueries;

        public WeatherController(
            IOpenWeatherService openWeatherService,
            IExecuteDatabaseQueries executeDatabaseQueries)
        {
            _openWeatherService = openWeatherService;
            _executeDatabaseQueries = executeDatabaseQueries;
        }

        [HttpGet]
        public async Task<ActionResult<List<Weather>>> GetAllWeather()
        {
            try
            {
                var weather = await _executeDatabaseQueries.GetAllWeatherAsync();
                return Ok(weather);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Weather>> GetWeatherById(int id)
        {
            try
            {
                var weather = await _executeDatabaseQueries.GetWeatherByIdAsync(id);
                if (weather == null)
                {
                    return NotFound($"Weather record with ID {id} not found");
                }
                return Ok(weather);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("coordinates")]
        public async Task<ActionResult<Weather>> GetWeatherByCoordinates(
            [FromQuery] decimal latitude, 
            [FromQuery] decimal longitude)
        {
            if (latitude < -90 || latitude > 90)
            {
                return BadRequest("Latitude must be between -90 and 90");
            }
            
            if (longitude < -180 || longitude > 180)
            {
                return BadRequest("Longitude must be between -180 and 180");
            }

            try
            {
                var existingWeather = await _executeDatabaseQueries.GetWeatherByCoordinatesAsync(latitude, longitude);
                
                if (existingWeather != null)
                {
                    return Ok(existingWeather);
                }

                var weather = await _openWeatherService.GetWeatherByCoordinatesAsync(latitude, longitude);
                
                if (weather == null)
                {
                    return NotFound($"Weather data for coordinates {latitude}, {longitude} not found");
                }

                var savedWeather = await _executeDatabaseQueries.SaveWeatherAsync(weather);
                return Ok(savedWeather);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }   
    }
}
