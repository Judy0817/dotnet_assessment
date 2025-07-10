USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'WeatherDB')
BEGIN
    ALTER DATABASE WeatherDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE WeatherDB;
END
GO

CREATE DATABASE WeatherDB;
GO

USE WeatherDB;
GO

CREATE TABLE Weather (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Latitude DECIMAL(9,6) NOT NULL,
    Longitude DECIMAL(9,6) NOT NULL,
    Temperature DECIMAL(5,2) NOT NULL,
    WeatherMain NVARCHAR(50) NOT NULL,
    WeatherDescription NVARCHAR(200) NOT NULL
);
GO

CREATE INDEX IX_Weather_Coordinates ON Weather (Latitude, Longitude);
CREATE INDEX IX_Weather_ID On Weather (Id);
GO
