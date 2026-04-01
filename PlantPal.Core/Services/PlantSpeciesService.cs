using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;

namespace PlantPal.Core.Services;

/// <summary>
/// Provides the built-in list of 40 common European houseplant species
/// with watering intervals, Wikipedia slugs, and bundled asset paths.
/// </summary>
public class PlantSpeciesService : IPlantSpeciesService
{
    private readonly IReadOnlyList<PlantSpecies> species;

    /// <summary>
    /// Initialises the service with the full 40-species list.
    /// </summary>
    public PlantSpeciesService()
    {
        this.species = BuildSpeciesList();
    }

    /// <inheritdoc />
    public IReadOnlyList<PlantSpecies> GetAll()
    {
        return this.species;
    }

    /// <inheritdoc />
    public PlantSpecies? FindByName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return this.species.FirstOrDefault(s =>
            s.CommonName.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    /// <inheritdoc />
    public int GetSuggestedInterval(string speciesKey)
    {
        var match = this.species.FirstOrDefault(s =>
            s.Id.Equals(speciesKey, StringComparison.OrdinalIgnoreCase));

        return match?.WateringIntervalDays ?? 7;
    }

    private static List<PlantSpecies> BuildSpeciesList()
    {
        return new List<PlantSpecies>
        {
            Create("monstera_deliciosa", "Monstera", "Monstera deliciosa", 7, "Monstera_deliciosa"),
            Create("pothos", "Pothos", "Epipremnum aureum", 7, "Epipremnum_aureum"),
            Create("snake_plant", "Snake Plant", "Dracaena trifasciata", 14, "Dracaena_trifasciata"),
            Create("peace_lily", "Peace Lily", "Spathiphyllum", 7, "Spathiphyllum"),
            Create("spider_plant", "Spider Plant", "Chlorophytum comosum", 7, "Chlorophytum_comosum"),
            Create("rubber_plant", "Rubber Plant", "Ficus elastica", 10, "Ficus_elastica"),
            Create("zz_plant", "ZZ Plant", "Zamioculcas zamiifolia", 14, "Zamioculcas"),
            Create("aloe_vera", "Aloe Vera", "Aloe vera", 14, "Aloe_vera"),
            Create("orchid", "Orchid", "Phalaenopsis", 7, "Phalaenopsis"),
            Create("calathea", "Calathea", "Calathea", 4, "Calathea"),
            Create("philodendron", "Philodendron", "Philodendron", 7, "Philodendron"),
            Create("dracaena", "Dracaena", "Dracaena", 10, "Dracaena_(plant)"),
            Create("english_ivy", "English Ivy", "Hedera helix", 5, "Hedera_helix"),
            Create("boston_fern", "Boston Fern", "Nephrolepis exaltata", 3, "Nephrolepis_exaltata"),
            Create("chinese_money_plant", "Chinese Money Plant", "Pilea peperomioides", 7, "Pilea_peperomioides"),
            Create("anthurium", "Anthurium", "Anthurium", 7, "Anthurium"),
            Create("bird_of_paradise", "Bird of Paradise", "Strelitzia reginae", 7, "Strelitzia_reginae"),
            Create("begonia", "Begonia", "Begonia", 5, "Begonia"),
            Create("yucca", "Yucca", "Yucca", 14, "Yucca"),
            Create("croton", "Croton", "Codiaeum variegatum", 5, "Codiaeum_variegatum"),
            Create("dumb_cane", "Dumb Cane", "Dieffenbachia", 7, "Dieffenbachia"),
            Create("hoya", "Hoya", "Hoya", 10, "Hoya_(plant)"),
            Create("prayer_plant", "Prayer Plant", "Maranta leuconeura", 5, "Maranta_leuconeura"),
            Create("umbrella_plant", "Umbrella Plant", "Schefflera", 7, "Schefflera"),
            Create("african_violet", "African Violet", "Streptocarpus sect. Saintpaulia", 4, "Saintpaulia"),
            Create("cyclamen", "Cyclamen", "Cyclamen", 5, "Cyclamen"),
            Create("christmas_cactus", "Christmas Cactus", "Schlumbergera", 10, "Schlumbergera"),
            Create("alocasia", "Alocasia", "Alocasia", 7, "Alocasia"),
            Create("peperomia", "Peperomia", "Peperomia", 10, "Peperomia"),
            Create("jade_plant", "Jade Plant", "Crassula ovata", 14, "Crassula_ovata"),
            Create("tradescantia", "Tradescantia", "Tradescantia", 5, "Tradescantia"),
            Create("weeping_fig", "Weeping Fig", "Ficus benjamina", 7, "Ficus_benjamina"),
            Create("areca_palm", "Areca Palm", "Dypsis lutescens", 5, "Dypsis_lutescens"),
            Create("parlour_palm", "Parlour Palm", "Chamaedorea elegans", 7, "Chamaedorea_elegans"),
            Create("cast_iron_plant", "Cast Iron Plant", "Aspidistra elatior", 14, "Aspidistra_elatior"),
            Create("monstera_adansonii", "Monstera Adansonii", "Monstera adansonii", 7, "Monstera_adansonii"),
            Create("fiddle_leaf_fig", "Fiddle Leaf Fig", "Ficus lyrata", 7, "Ficus_lyrata"),
            Create("echeveria", "Echeveria", "Echeveria", 14, "Echeveria"),
            Create("cactus_mix", "Cactus Mix", "Cactaceae", 14, "Cactus"),
            Create("flamingo_flower", "Flamingo Flower", "Anthurium andraeanum", 7, "Anthurium_andraeanum"),
        };
    }

    private static PlantSpecies Create(string id, string commonName, string latinName, int intervalDays, string wikiSlug)
    {
        return new PlantSpecies
        {
            Id = id,
            CommonName = commonName,
            LatinName = latinName,
            WateringIntervalDays = intervalDays,
            WikipediaSlug = wikiSlug,
            ThumbnailAssetPath = $"plants/{id}_thumb.png",
            DetailAssetPath = $"plants/{id}_detail.png",
        };
    }
}
