namespace PlantPal.Core.Interfaces;

/// <summary>
/// Abstracts platform secure storage so it can be mocked in unit tests.
/// On device this wraps the encrypted keychain (Android Keystore / iOS Keychain).
/// </summary>
public interface ISecureStorageService
{
    /// <summary>Returns the value stored under <paramref name="key"/>, or null if not found.</summary>
    Task<string?> GetAsync(string key);

    /// <summary>Stores <paramref name="value"/> under <paramref name="key"/>.</summary>
    Task SetAsync(string key, string value);

    /// <summary>Removes the value stored under <paramref name="key"/>. Returns true if it existed.</summary>
    Task<bool> RemoveAsync(string key);
}
