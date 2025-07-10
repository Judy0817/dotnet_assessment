# Weather API

## Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- SQL Server (with instance name MSSQLSERVER01)
- OpenWeatherMap API key
    - First create openweather map account using "https://home.openweathermap.org/users/sign_in"
    - Then you can get your api key from "https://home.openweathermap.org/api_keys" 
    - Then create .env file as .env.example file and add api key

### Installation Steps

1. **Set up the database**:
   ```powershell
   sqlcmd -S localhost\MSSQLSERVER01 -i "setup.sql"
   ```

2. **Navigate to the project directory**:
   ```powershell
   cd WeatherApi
   ```

3. **Restore dependencies**:
   ```powershell
   dotnet restore
   ```

4. **Run the application**:
   ```powershell
   dotnet run
   ```
  
## Technologies & Libraries Used

### Core Technologies
- **ASP.NET Core 8.0** - Modern, cross-platform web framework for building APIs
- **C#** - As a programming language
- **SQL Server** - As a relational database
- **Entity Framework Core** - Database operations

### Key Libraries & Packages
- **System.Data.SqlClient (4.8.6)** - SQL Server connectivity and data access
- **HttpClient** - HTTP client for external API calls to OpenWeatherMap
- **Swashbuckle.AspNetCore (6.6.2)** - Swagger UI integration for interactive API documentation and testing

## API Endpoints

### Base URL
`http://localhost:5044`

### Available Endpoints

#### 1. Get All Weather Records
```
GET /api/weather
```
**Description**: Retrieves all weather records stored in the database.

**Response Example**:
```json
[
  {
    "id": 1,
    "latitude": 40.7128,
    "longitude": -74.0060,
    "temperature": 22.5,
    "weatherMain": "Clouds",
    "weatherDescription": "broken clouds"
  },
  {
    "id": 2,
    "latitude": 51.5074,
    "longitude": -0.1278,
    "temperature": 18.3,
    "weatherMain": "Rain",
    "weatherDescription": "light rain"
  }
]
```

**Status Codes**:
- `200 OK` - Success
- `500 Internal Server Error` - Server error

#### 2. Get Weather by ID
```
GET /api/weather/{id}
```
**Description**: Retrieves a specific weather record by its ID.

**Parameters**:
- `id` (int, required) - The weather record ID

**Response Example**:
```json
{
  "id": 1,
  "latitude": 40.7128,
  "longitude": -74.0060,
  "temperature": 22.5,
  "weatherMain": "Clouds",
  "weatherDescription": "broken clouds"
}
```

**Status Codes**:
- `200 OK` - Success
- `404 Not Found` - Weather record not found
- `500 Internal Server Error` - Server error

#### 3. Get Weather by Coordinates
```
GET /api/weather/coordinates?latitude={lat}&longitude={lon}
```
**Description**: Retrieves weather data for specific coordinates. If data exists in the database, it returns data. Otherwise, it fetches fresh data from OpenWeatherMap API and stores it.

**Query Parameters**:
- `latitude` (decimal, required) - Latitude coordinate (-90 to 90)
- `longitude` (decimal, required) - Longitude coordinate (-180 to 180)

**Example Request**:
```
GET /api/weather/coordinates?latitude=40.7128&longitude=-74.0060
```

**Response Example**:
```json
{
  "id": 1,
  "latitude": 40.7128,
  "longitude": -74.0060,
  "temperature": 22.5,
  "weatherMain": "Clouds",
  "weatherDescription": "broken clouds"
}
```

**Status Codes**:
- `200 OK` - Success
- `400 Bad Request` - Invalid latitude or longitude values
- `404 Not Found` - Weather data not available for coordinates
- `500 Internal Server Error` - Server error


## Testing the API

### Example cURL Commands

```bash
# Get all weather records
curl -X GET "https://localhost:5044/api/weather"

# Get weather by ID
curl -X GET "https://localhost:5044/api/weather/1"

# Get weather by coordinates (New York City)
curl -X GET "https://localhost:5044/api/weather/coordinates?latitude=40.7128&longitude=-74.0060"
```

