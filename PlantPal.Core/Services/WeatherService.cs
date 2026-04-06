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
    // Precipitation thresholds in millimetres for postponement decisions.
    // > 10 mm = heavy rain (typical thunderstorm or sustained rain) → postpone 2 days.
    // > 5 mm  = light rain (drizzle or short shower)               → postpone 1 day.
    // ≤ 5 mm  = dry or trace precipitation                         → no postponement.
    // These values were chosen to avoid unnecessary watering after significant outdoor rainfall
    // while still reminding users to water after light showers that may not fully saturate the soil.
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

    /// <summary>
    /// Extracts today's precipitation total (mm) from an Open-Meteo daily forecast response.
    /// </summary>
    /// <remarks>
    /// The request uses <c>past_days=1&amp;forecast_days=1</c>, so the API returns exactly
    /// two values in the <c>precipitation_sum</c> array: index 0 = today, index 1 = tomorrow.
    /// We read index 0 (today) because we want to know whether it has already rained,
    /// not whether it is forecast to rain. This prevents premature postponement on sunny days
    /// where rain is predicted but has not yet fallen.
    /// Throws if the JSON is malformed; the caller catches all exceptions and returns 0.
    /// </remarks>
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
