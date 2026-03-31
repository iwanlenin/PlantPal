namespace PlantPal.Core.Models;

/// <summary>
/// Represents a known plant species with care metadata and image asset references.
/// </summary>
public class PlantSpecies
{
    /// <summary>Gets or sets the unique string key for this species (e.g. "monstera_deliciosa").</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets the common name of the species (e.g. "Monstera").</summary>
    public string CommonName { get; set; } = string.Empty;

    /// <summary>Gets or sets the Latin scientific name of the species (e.g. "Monstera deliciosa").</summary>
    public string LatinName { get; set; } = string.Empty;

    /// <summary>Gets or sets the suggested number of days between waterings for this species.</summary>
    public int WateringIntervalDays { get; set; }

    /// <summary>Gets or sets the Wikipedia page slug used to fetch detail images (e.g. "Monstera_deliciosa").</summary>
    public string WikipediaSlug { get; set; } = string.Empty;

    /// <summary>Gets or sets the path to the bundled thumbnail asset (e.g. "plants/monstera_deliciosa_thumb.png").</summary>
    public string ThumbnailAssetPath { get; set; } = string.Empty;

    /// <summary>Gets or sets the path to the bundled detail image asset (e.g. "plants/monstera_deliciosa_detail.png").</summary>
    public string DetailAssetPath { get; set; } = string.Empty;
}
