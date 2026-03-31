namespace PlantPal.Core.Interfaces;

/// <summary>
/// Defines the contract for resolving plant species images.
/// Thumbnails are always available offline (bundled). Detail images are downloaded on first use and cached locally.
/// Never throws on missing image — always returns a valid fallback path.
/// </summary>
public interface IImageCacheService
{
    /// <summary>
    /// Returns the local path to the bundled thumbnail image for the specified species key.
    /// Always succeeds — thumbnails are bundled in the app package.
    /// </summary>
    /// <param name="speciesKey">The species identifier (e.g. "monstera_deliciosa").</param>
    Task<string> GetThumbnailPathAsync(string speciesKey);

    /// <summary>
    /// Returns the local path to the detail image for the specified species key.
    /// Downloads and caches the image on first call. Falls back to the thumbnail if offline or on error.
    /// </summary>
    /// <param name="speciesKey">The species identifier (e.g. "monstera_deliciosa").</param>
    Task<string> GetDetailImageAsync(string speciesKey);
}
