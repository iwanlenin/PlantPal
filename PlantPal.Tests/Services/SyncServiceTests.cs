using NSubstitute;
using NSubstitute.ExceptionExtensions;
using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;
using PlantPal.Core.Services;

namespace PlantPal.Tests.Services;

/// <summary>Unit tests for <see cref="SyncService"/>.</summary>
public class SyncServiceTests
{
    private readonly IPlantRepository plantRepository = Substitute.For<IPlantRepository>();
    private readonly IWateringLogRepository wateringLogRepository = Substitute.For<IWateringLogRepository>();
    private readonly IConnectivityService connectivityService = Substitute.For<IConnectivityService>();
    private readonly ISupabaseClient supabaseClient = Substitute.For<ISupabaseClient>();

    private SyncService CreateSut() =>
        new(this.plantRepository, this.wateringLogRepository, this.connectivityService, this.supabaseClient);

    [Fact]
    public async Task SyncUpAsync_WhenOnlineAndSignedIn_UpsertsPlantsToSupabase()
    {
        // Arrange
        this.connectivityService.IsConnected.Returns(true);
        this.supabaseClient.IsSignedIn.Returns(true);
        this.plantRepository.GetAllAsync().Returns(new List<Plant>
        {
            new() { Id = 1, SyncId = "uuid-1", Name = "Basil", Species = "Ocimum basilicum", Location = "Kitchen", WateringIntervalDays = 2, UpdatedAt = DateTime.UtcNow },
            new() { Id = 2, SyncId = "uuid-2", Name = "Fern", Species = "Boston Fern", Location = "Living Room", WateringIntervalDays = 3, UpdatedAt = DateTime.UtcNow },
        });
        this.wateringLogRepository.GetAllAsync().Returns(new List<WateringLog>());

        var sut = this.CreateSut();

        // Act
        await sut.SyncUpAsync();

        // Assert
        await this.supabaseClient.Received(1).UpsertPlantsAsync(
            Arg.Is<IList<PlantSyncRecord>>(list => list.Count == 2));
        Assert.Equal(SyncStatus.Idle, sut.Status);
        Assert.NotNull(sut.LastSyncedAt);
    }

    [Fact]
    public async Task SyncDownAsync_WhenRemotePlantIsNewer_UpdatesLocalPlant()
    {
        // Arrange
        this.connectivityService.IsConnected.Returns(true);
        this.supabaseClient.IsSignedIn.Returns(true);

        var oldTime = DateTime.UtcNow.AddHours(-2);
        var newTime = DateTime.UtcNow;

        this.plantRepository.GetAllAsync().Returns(new List<Plant>
        {
            new() { Id = 1, SyncId = "uuid-1", Name = "Old Name", Species = "Basil", Location = "Kitchen", WateringIntervalDays = 2, UpdatedAt = oldTime },
        });
        this.supabaseClient.FetchPlantsAsync().Returns(new List<PlantSyncRecord>
        {
            new() { SyncId = "uuid-1", Name = "New Name", Species = "Basil", Location = "Kitchen", WateringIntervalDays = 2, UpdatedAt = newTime },
        });
        this.supabaseClient.FetchWateringLogsAsync().Returns(new List<WateringLogSyncRecord>());
        this.wateringLogRepository.GetAllAsync().Returns(new List<WateringLog>());

        var sut = this.CreateSut();

        // Act
        await sut.SyncDownAsync();

        // Assert — local plant updated with newer remote name
        await this.plantRepository.Received(1).SaveAsync(
            Arg.Is<Plant>(p => p.Name == "New Name" && p.SyncId == "uuid-1"));
        Assert.Equal(SyncStatus.Idle, sut.Status);
    }

    [Fact]
    public async Task SyncUpAsync_WhenOffline_SetsStatusOfflineWithoutThrowing()
    {
        // Arrange
        this.connectivityService.IsConnected.Returns(false);
        var sut = this.CreateSut();

        // Act
        await sut.SyncUpAsync();

        // Assert
        Assert.Equal(SyncStatus.Offline, sut.Status);
        await this.supabaseClient.DidNotReceive().UpsertPlantsAsync(Arg.Any<IList<PlantSyncRecord>>());
    }

    [Fact]
    public async Task SyncDownAsync_WhenSupabaseThrows_SetsStatusErrorWithoutCorruptingLocal()
    {
        // Arrange
        this.connectivityService.IsConnected.Returns(true);
        this.supabaseClient.IsSignedIn.Returns(true);
        this.plantRepository.GetAllAsync().Returns(new List<Plant>
        {
            new() { Id = 1, SyncId = "uuid-1", Name = "Basil", Species = "Basil", Location = "Kitchen", WateringIntervalDays = 2, UpdatedAt = DateTime.UtcNow },
        });
        this.supabaseClient.FetchPlantsAsync().ThrowsAsync(new HttpRequestException("500 Internal Server Error"));

        var sut = this.CreateSut();

        // Act — must not throw
        await sut.SyncDownAsync();

        // Assert
        Assert.Equal(SyncStatus.Error, sut.Status);
        await this.plantRepository.DidNotReceive().SaveAsync(Arg.Any<Plant>());
    }

    [Fact]
    public async Task SyncDownAsync_WhenNoRemoteChanges_CompletesWithoutModifyingLocal()
    {
        // Arrange
        this.connectivityService.IsConnected.Returns(true);
        this.supabaseClient.IsSignedIn.Returns(true);
        this.plantRepository.GetAllAsync().Returns(new List<Plant>());
        this.supabaseClient.FetchPlantsAsync().Returns(new List<PlantSyncRecord>());
        this.supabaseClient.FetchWateringLogsAsync().Returns(new List<WateringLogSyncRecord>());
        this.wateringLogRepository.GetAllAsync().Returns(new List<WateringLog>());

        var sut = this.CreateSut();

        // Act
        await sut.SyncDownAsync();

        // Assert
        await this.plantRepository.DidNotReceive().SaveAsync(Arg.Any<Plant>());
        Assert.Equal(SyncStatus.Idle, sut.Status);
        Assert.NotNull(sut.LastSyncedAt);
    }
}
