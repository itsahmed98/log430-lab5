using System.Net;
using System.Net.Http.Json;
using AdministrationMcService.Models;
using AdministrationMcService.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace AdministrationMcService.Test.UnitTests
{
    public class RapportApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        public RapportApiIntegrationTests(WebApplicationFactory<Program> factory)
            => _factory = factory;

        [Fact]
        public async Task GetRapport_ShouldReturnOkWithDto_WhenServiceReturnsData()
        {
            // Arrange : on override le service
            var fakeDto = new RapportVentesDto();
            var factory = _factory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IRapportService>();
                    var mock = new Mock<IRapportService>();
                    mock.Setup(s => s.ObtenirRapportConsolideAsync())
                        .ReturnsAsync(fakeDto);
                    services.AddSingleton(mock.Object);
                }));

            var client = factory.CreateClient();

            // Act
            var resp = await client.GetAsync("/api/v1/administration/rapports");

            // Assert
            resp.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await resp.Content.ReadFromJsonAsync<RapportVentesDto>();
            result.Should().BeEquivalentTo(fakeDto);
        }

        [Fact]
        public async Task GetRapport_ShouldReturnNotFound_WhenServiceReturnsNull()
        {
            // Arrange
            var factory = _factory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IRapportService>();
                    var mock = new Mock<IRapportService>();
                    mock.Setup(s => s.ObtenirRapportConsolideAsync())
                        .ReturnsAsync((RapportVentesDto?)null);
                    services.AddSingleton(mock.Object);
                }));

            var client = factory.CreateClient();

            // Act
            var resp = await client.GetAsync("/api/v1/administration/rapports");

            // Assert
            resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
