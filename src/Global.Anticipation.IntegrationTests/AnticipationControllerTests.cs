using FluentAssertions;
using Global.Anticipation.API.Transport;
using Global.Anticipation.Application.Features.Anticipation.Commands.CreateAnticipation;
using Global.Anticipation.Application.Features.Anticipation.Queries.GetAnticipationById;
using Global.Anticipation.Domain.Entities;
using Global.Anticipation.Infra.Data.Persistence;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Global.Anticipation.API.Tests.Controllers
{
    public class AnticipationControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly JsonSerializerOptions _jsonOptions;

        public AnticipationControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            _jsonOptions.Converters.Add(new JsonStringEnumConverter());
        }

        [Fact]
        public async Task Create_WithValidData_ShouldReturnCreated()
        {
            // Arrange
            var request = new CreateAnticipationRequest
            (
                Guid.NewGuid(),
                1500m,
                DateTime.UtcNow
            );

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/anticipations", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await response.Content.ReadFromJsonAsync<CreateAnticipationOutput>(_jsonOptions);
            content.Should().NotBeNull();
            content?.Status.Should().Be(Status.Pending);
        }

        [Fact]
        public async Task GetById_WhenAnticipationExists_ShouldReturnOk()
        {
            // Arrange
            var entity = new AnticipationEntity(Guid.NewGuid(), 1200m, DateTime.UtcNow);
            await SeedDatabaseAsync(entity); // Adiciona a entidade ao banco em memória

            // Act
            var response = await _client.GetAsync($"/api/v1/anticipations/{entity.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<GetAnticipationByIdOutput>(_jsonOptions);
            content.Should().NotBeNull();
            content?.Id.Should().Be(entity.Id);
            content?.RequestedAmount.Should().Be(entity.RequestedAmount);
        }

        [Fact]
        public async Task GetById_WhenAnticipationDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/v1/anticipations/{nonExistentId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateStatus_WithValidData_ShouldReturnOk()
        {
            // Arrange
            var entity = new AnticipationEntity(Guid.NewGuid(), 1000m, DateTime.UtcNow);
            await SeedDatabaseAsync(entity);
            var dto = new UpdateStatusRequest(StatusRequest.Approved);

            // Act
            var response = await _client.PatchAsJsonAsync($"/api/v1/anticipations/{entity.Id}/status", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var updatedEntity = await FindEntityInDbAsync(entity.Id);
            updatedEntity?.Status.Should().Be(Status.Approved);
        }

        private async Task SeedDatabaseAsync(params AnticipationEntity[] entities)
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AnticipationContext>();
            await context.Anticipation.AddRangeAsync(entities);
            await context.SaveChangesAsync();
        }

        private async Task<AnticipationEntity?> FindEntityInDbAsync(Guid id)
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AnticipationContext>();
            return await context.Anticipation.FindAsync(id);
        }
    }
}