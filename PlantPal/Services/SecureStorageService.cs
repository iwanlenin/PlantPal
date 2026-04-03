using PlantPal.Core.Interfaces;

namespace PlantPal.Services;

/// <summary>
/// <see cref="ISecureStorageService"/> implementation backed by MAUI <see cref="SecureStorage"/>.
/// Values are stored in the platform encrypted keychain (Android Keystore / iOS Keychain).
/// </summary>
public class SecureStorageService : ISecureStorageService
{
    /// <inheritdoc />
    public async Task<string?> GetAsync(string key) =>
        await SecureStorage.GetAsync(key);

    /// <inheritdoc />
    public async Task SetAsync(string key, string value) =>
        await SecureStorage.SetAsync(key, value);

    /// <inheritdoc />
    public Task<bool> RemoveAsync(string key) =>
        Task.FromResult(SecureStorage.Remove(key));
}
