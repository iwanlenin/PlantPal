using PlantPal.Core.Interfaces;

namespace PlantPal.Services;

/// <summary>
/// Concrete <see cref="IConnectivityService"/> implementation wrapping the MAUI Connectivity API.
/// </summary>
public class ConnectivityService : IConnectivityService
{
    /// <inheritdoc />
    public bool IsConnected =>
        Connectivity.NetworkAccess == NetworkAccess.Internet;
}
