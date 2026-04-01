using PlantPal.Core.Interfaces;
using PlantPal.Core.Services;

namespace PlantPal.Tests.Services;

/// <summary>
/// Tests for <see cref="PlantSpeciesService"/> covering the full 40-species list,
/// case-insensitive search, and default fallback behavior.
/// </summary>
public class PlantSpeciesServiceTests
{
    private readonly IPlantSpeciesService service = new PlantSpeciesService();

    [Fact]
    public void GetAll_Returns_Exactly_40_Species()
    {
        var species = this.service.GetAll();

        Assert.Equal(40, species.Count);
    }

    [Fact]
    public void FindByName_Monstera_Returns_WateringInterval_7()
    {
        var result = this.service.FindByName("Monstera");

        Assert.NotNull(result);
        Assert.Equal(7, result.WateringIntervalDays);
    }

    [Fact]
    public void FindByName_Is_CaseInsensitive()
    {
        var lower = this.service.FindByName("monstera");
        var upper = this.service.FindByName("MONSTERA");
        var mixed = this.service.FindByName("mOnStErA");

        Assert.NotNull(lower);
        Assert.NotNull(upper);
        Assert.NotNull(mixed);
        Assert.Equal(lower.Id, upper.Id);
        Assert.Equal(lower.Id, mixed.Id);
    }

    [Fact]
    public void GetSuggestedInterval_KnownKey_Returns_CorrectValue()
    {
        int interval = this.service.GetSuggestedInterval("cactus_mix");

        Assert.Equal(14, interval);
    }

    [Fact]
    public void GetAll_All_Species_Have_NonNull_WikipediaSlug()
    {
        var species = this.service.GetAll();

        Assert.All(species, s => Assert.False(string.IsNullOrWhiteSpace(s.WikipediaSlug),
            $"Species '{s.Id}' has null or empty WikipediaSlug"));
    }

    [Fact]
    public void GetAll_All_Species_Have_NonNull_ThumbnailAssetPath()
    {
        var species = this.service.GetAll();

        Assert.All(species, s => Assert.False(string.IsNullOrWhiteSpace(s.ThumbnailAssetPath),
            $"Species '{s.Id}' has null or empty ThumbnailAssetPath"));
    }

    [Fact]
    public void FindByName_NonExistent_Returns_Null()
    {
        var result = this.service.FindByName("NonExistentPlant");

        Assert.Null(result);
    }

    [Fact]
    public void FindByName_Null_Returns_Null()
    {
        var result = this.service.FindByName(null);

        Assert.Null(result);
    }

    [Fact]
    public void FindByName_Empty_Returns_Null()
    {
        var result = this.service.FindByName("");

        Assert.Null(result);
    }

    [Fact]
    public void GetSuggestedInterval_UnknownKey_Returns_7()
    {
        int interval = this.service.GetSuggestedInterval("completely_unknown_species");

        Assert.Equal(7, interval);
    }
}
