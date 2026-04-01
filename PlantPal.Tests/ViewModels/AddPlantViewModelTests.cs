using NSubstitute;
using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;
using PlantPal.Core.ViewModels;

namespace PlantPal.Tests.ViewModels;

/// <summary>
/// Unit tests for <see cref="AddPlantViewModel"/> using NSubstitute mocks.
/// </summary>
public class AddPlantViewModelTests
{
    private readonly IPlantRepository repository = Substitute.For<IPlantRepository>();
    private readonly IPlantSpeciesService speciesService = Substitute.For<IPlantSpeciesService>();
    private readonly INavigationService navigationService = Substitute.For<INavigationService>();

    private AddPlantViewModel CreateViewModel() =>
        new(this.repository, this.speciesService, this.navigationService);

    // ── Positive cases ─────────────────────────────────────────────────────────

    [Fact]
    public async Task SaveAsync_ValidName_Calls_Repository_SaveAsync()
    {
        var vm = this.CreateViewModel();
        vm.Name = "My Monstera";
        vm.Species = "Monstera";
        vm.WateringIntervalDays = 7;

        await vm.SaveCommand.ExecuteAsync(null);

        await this.repository.Received(1).SaveAsync(Arg.Is<Plant>(p => p.Name == "My Monstera"));
    }

    [Fact]
    public async Task SaveAsync_ValidName_Navigates_Back()
    {
        var vm = this.CreateViewModel();
        vm.Name = "My Monstera";

        await vm.SaveCommand.ExecuteAsync(null);

        await this.navigationService.Received(1).GoBackAsync();
    }

    [Fact]
    public void SelectedSpecies_Updates_WateringIntervalDays()
    {
        var species = new PlantSpecies
        {
            Id = "monstera_deliciosa",
            CommonName = "Monstera",
            WateringIntervalDays = 7,
        };
        var vm = this.CreateViewModel();

        vm.SelectedSpecies = species;

        Assert.Equal(7, vm.WateringIntervalDays);
    }

    [Fact]
    public void SelectedSpecies_Updates_Species_Name()
    {
        var species = new PlantSpecies
        {
            Id = "monstera_deliciosa",
            CommonName = "Monstera",
            WateringIntervalDays = 7,
        };
        var vm = this.CreateViewModel();

        vm.SelectedSpecies = species;

        Assert.Equal("Monstera", vm.Species);
    }

    [Fact]
    public void SearchText_Filters_Species_List()
    {
        this.speciesService.GetAll().Returns(new List<PlantSpecies>
        {
            new() { Id = "monstera_deliciosa", CommonName = "Monstera", WateringIntervalDays = 7 },
            new() { Id = "cactus_mix", CommonName = "Cactus Mix", WateringIntervalDays = 14 },
        });
        var vm = this.CreateViewModel();

        vm.SearchText = "Mon";

        Assert.Single(vm.FilteredSpecies);
        Assert.Equal("Monstera", vm.FilteredSpecies[0].CommonName);
    }

    [Fact]
    public async Task LoadPlantAsync_EditMode_Prefills_Fields()
    {
        var plant = new Plant
        {
            Id = 5,
            Name = "Office Fern",
            Species = "Boston Fern",
            Location = "Kitchen",
            WateringIntervalDays = 3,
            LastWateredDate = new DateTime(2026, 3, 25),
        };
        this.repository.GetByIdAsync(5).Returns(plant);
        var vm = this.CreateViewModel();

        await vm.LoadPlantAsync(5);

        Assert.Equal("Office Fern", vm.Name);
        Assert.Equal("Boston Fern", vm.Species);
        Assert.Equal("Kitchen", vm.Location);
        Assert.Equal(3, vm.WateringIntervalDays);
        Assert.True(vm.IsEditMode);
    }

    [Fact]
    public async Task SaveAsync_EditMode_Updates_Existing_Plant()
    {
        var plant = new Plant { Id = 5, Name = "Old Name", Species = "Monstera", WateringIntervalDays = 7 };
        this.repository.GetByIdAsync(5).Returns(plant);
        var vm = this.CreateViewModel();
        await vm.LoadPlantAsync(5);

        vm.Name = "New Name";
        await vm.SaveCommand.ExecuteAsync(null);

        await this.repository.Received(1).SaveAsync(Arg.Is<Plant>(p => p.Id == 5 && p.Name == "New Name"));
    }

    [Fact]
    public async Task DeleteAsync_Calls_Repository_DeleteAsync()
    {
        var plant = new Plant { Id = 5, Name = "Doomed Plant", Species = "Monstera", WateringIntervalDays = 7 };
        this.repository.GetByIdAsync(5).Returns(plant);
        var vm = this.CreateViewModel();
        await vm.LoadPlantAsync(5);

        await vm.DeleteCommand.ExecuteAsync(null);

        await this.repository.Received(1).DeleteAsync(5);
    }

    // ── Negative cases ─────────────────────────────────────────────────────────

    [Fact]
    public async Task SaveAsync_EmptyName_Does_Not_Call_Repository()
    {
        var vm = this.CreateViewModel();
        vm.Name = "";

        await vm.SaveCommand.ExecuteAsync(null);

        await this.repository.DidNotReceive().SaveAsync(Arg.Any<Plant>());
    }

    [Fact]
    public async Task SaveAsync_EmptyName_Sets_NameError()
    {
        var vm = this.CreateViewModel();
        vm.Name = "";

        await vm.SaveCommand.ExecuteAsync(null);

        Assert.False(string.IsNullOrEmpty(vm.NameError));
    }

    [Fact]
    public void IsEditMode_False_By_Default()
    {
        var vm = this.CreateViewModel();

        Assert.False(vm.IsEditMode);
    }

    [Fact]
    public async Task DeleteAsync_NotEditMode_Does_Not_Call_Repository()
    {
        var vm = this.CreateViewModel();

        await vm.DeleteCommand.ExecuteAsync(null);

        await this.repository.DidNotReceive().DeleteAsync(Arg.Any<int>());
    }
}
