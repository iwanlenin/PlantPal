namespace PlantPal.Core.Interfaces;

/// <summary>
/// Provides weather-based watering postponement data from the Open-Meteo API.
/// All methods are safe to call when offline or when coordinates have not been set — they return 0.
/// </summary>
public interface IWeatherService
{
    /// <summary>
    /// Stores the device's current coordinates for use in weather queries.
    /// Must be called before <see cref="GetPostponementDaysAsync"/> returns a non-zero value.
    /// </summary>
    /// <param name="latitude">WGS-84 latitude in decimal degrees.</param>
    /// <param name="longitude">WGS-84 longitude in decimal degrees.</param>
    void SetCoordinates(double latitude, double longitude);

    /// <summary>
    /// Returns how many days to postpone a watering reminder based on today's rainfall.
    /// Returns 0 if offline, coordinates are not set, or the API call fails for any reason.
    /// </summary>
    Task<int> GetPostponementDaysAsync();
}
