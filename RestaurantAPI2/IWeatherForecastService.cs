using System.Collections.Generic;

namespace RestaurantAPI2
{
    public interface IWeatherForecastService
    {
        IEnumerable<WeatherForecast> Get();
        IEnumerable<WeatherForecast> Get(int numberOfResults, int minTemp, int maxTemp);
    }
}