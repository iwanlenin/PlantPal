namespace PlantPal.Core.Interfaces;

/// <summary>
/// Defines the contract for Shell-based navigation within the app.
/// ViewModels must use this interface rather than calling Shell.Current directly.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Navigates to the specified Shell route, optionally passing query parameters.
    /// </summary>
    /// <param name="route">The registered Shell route to navigate to (e.g. "//Dashboard", "AddPlant").</param>
    /// <param name="parameters">Optional navigation parameters passed as query string values.</param>
    Task NavigateToAsync(string route, Dictionary<string, object>? parameters = null);

    /// <summary>Navigates back to the previous page in the navigation stack.</summary>
    Task GoBackAsync();
}
