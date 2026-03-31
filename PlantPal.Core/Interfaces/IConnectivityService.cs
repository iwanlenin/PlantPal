namespace PlantPal.Core.Interfaces;

/// <summary>
/// Defines the contract for checking network connectivity.
/// Wraps the platform network API so it can be mocked in tests.
/// </summary>
public interface IConnectivityService
{
    /// <summary>Gets a value indicating whether the device currently has internet access.</summary>
    bool IsConnected { get; }
}
