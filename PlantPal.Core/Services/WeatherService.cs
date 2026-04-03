using System.Text.Json;
using PlantPal.Core.Interfaces;

namespace PlantPal.Core.Services;

/// <summary>
/// Fetches daily precipitation data from the Open-Meteo API and converts it into
/// a watering postponement value in days.
/// Always returns 0 on any failure — never throws.
/// </summary>
public class WeatherService : IWeatherService
{
    private const double RainThresholdHeavy = 10.0;
    private const double RainThresholdLight = 5.0;

    private readonly IHttpClientWrapper http;
    private readonly IConnectivityService connectivity;

    private double latitude;
    private double longitude;
    private bool coordinatesSet;

    /// <summary>
    /// Initialises a new instance of <see cref="WeatherService"/>.
    /// </summary>
    /// <param name="http">HTTP client wrapper for making API requests.</param>
    /// <param name="connectivity">Connectivity service to check network state before calling the API.</param>
    public WeatherService(IHttpClientWrapper http, IConnectivityService connectivity)
    {
        this.http = http;
        this.connectivity = connectivity;
    }

    /// <inheritdoc />
    public void SetCoordinates(double latitude, double longitude)
    {
        this.latitude = latitude;
        this.longitude = longitude;
        this.coordinatesSet = true;
    }

    /// <inheritdoc />
    public async Task<int> GetPostponementDaysAsync()
    {
        if (!this.coordinatesSet)
        {
            return 0;
        }

        if (!this.connectivity.IsConnected)
        {
            return 0;
        }

        try
        {
            var url = $"https://api.open-meteo.com/v1/forecast" +
                      $"?latitude={this.latitude}&longitude={this.longitude}" +
                      $"&daily=precipitation_sum&forecast_days=1&past_days=1";

            var json = await this.http.GetStringAsync(url);
            var precipitation = this.ParsePrecipitation(json);

            if (precipitation > RainThresholdHeavy)
            {
                return 2;
            }

            if (precipitation > RainThresholdLight)
            {
                return 1;
            }

            return 0;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>Parses the first precipitation_sum value from an Open-Meteo daily response.</summary>
    private double ParsePrecipitation(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var values = doc.RootElement
            .GetProperty("daily")
            .GetProperty("precipitation_sum")
            .EnumerateArray();

        return values.First().GetDouble();
    }
}
