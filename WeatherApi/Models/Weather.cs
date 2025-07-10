namespace WeatherApi.Models
{
    public class Weather
    {
        public int Id { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Temperature { get; set; }
        public string WeatherMain { get; set; } = string.Empty;
        public string WeatherDescription { get; set; } = string.Empty;
    }

    // GenAI
    public class OpenWeatherResponse
    {
        public Coord Coord { get; set; } = new Coord();
        public WeatherInfo[] Weather { get; set; } = Array.Empty<WeatherInfo>();
        public string Base { get; set; } = string.Empty;
        public Main Main { get; set; } = new Main();
        public int Visibility { get; set; }
        public Wind? Wind { get; set; }
        public Rain? Rain { get; set; }
        public Clouds? Clouds { get; set; }
        public long Dt { get; set; }
        public Sys Sys { get; set; } = new Sys();
        public int Timezone { get; set; }
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Cod { get; set; }
    }

    public class Coord
    {
        public decimal Lon { get; set; }
        public decimal Lat { get; set; }
    }

    public class Main
    {
        public decimal Temp { get; set; }
        public decimal Feels_Like { get; set; }
        public decimal Temp_Min { get; set; }
        public decimal Temp_Max { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
        public int? Sea_Level { get; set; }
        public int? Grnd_Level { get; set; }
    }

    public class WeatherInfo
    {
        public int Id { get; set; }
        public string Main { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }

    public class Wind
    {
        public decimal Speed { get; set; }
        public int Deg { get; set; }
        public decimal? Gust { get; set; }
    }

    public class Rain
    {
        public decimal OneH { get; set; }
    }

    public class Clouds
    {
        public int All { get; set; }
    }

    public class Sys
    {
        public int Type { get; set; }
        public long Id { get; set; }
        public string Country { get; set; } = string.Empty;
        public long Sunrise { get; set; }
        public long Sunset { get; set; }
    }
}
