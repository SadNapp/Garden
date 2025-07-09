using ConsoleTest;
using System;

namespace ConsoleTest
{
    // Перерахування для типів погоди
    enum WeatherType
    {
        Sunny,
        Rainy,
        Drought,
        Foggy
    }

    class WeatherSystem
    {
        // Змінено set-аксесор на public для можливості завантаження стану
        public WeatherType CurrentWeather { get; set; }

        public WeatherSystem()
        {
            CurrentWeather = WeatherType.Sunny; // Початкова погода
        }

        public void GenerateNewWeather()
        {
            CurrentWeather = (WeatherType)Program.random.Next(0, Enum.GetValues(typeof(WeatherType)).Length);
        }

        public string GetWeatherName(WeatherType weather)
        {
            switch (weather)
            {
                case WeatherType.Sunny: return "Сонячно";
                case WeatherType.Rainy: return "Дощ";
                case WeatherType.Drought: return "Засуха";
                case WeatherType.Foggy: return "Туман";
                default: return "Невідомо";
            }
        }

        public string GetWeatherASCII(WeatherType weather)
        {
            switch (weather)
            {
                case WeatherType.Sunny: return "☀️";
                case WeatherType.Rainy: return "🌧️";
                case WeatherType.Drought: return "🏜️";
                case WeatherType.Foggy: return "🌫️";
                default: return "";
            }
        }

        public ConsoleColor GetWeatherColor(WeatherType weather)
        {
            switch (weather)
            {
                case WeatherType.Sunny: return ConsoleColor.Yellow;
                case WeatherType.Rainy: return ConsoleColor.Blue;
                case WeatherType.Drought: return ConsoleColor.DarkRed;
                case WeatherType.Foggy: return ConsoleColor.DarkGray;
                default: return ConsoleColor.Gray;
            }
        }
    }
}