using AdministrationMcService.Data;
using AdministrationMcService.Models;
using AdministrationMcService.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;


namespace AdministrationMcService.Test.UnitTests.Services
{
    public class MagasinServiceTests
    {
        private AdminDbContext CreateContextWithData()
        {
            var options = new DbContextOptionsBuilder<AdminDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var ctx = new AdminDbContext(options);
            ctx.Magasins.AddRange(new[]
            {
                new Magasin { MagasinId = 1, Nom = "Mag1" },
                new Magasin { MagasinId = 2, Nom = "Mag2" },
            });
            ctx.SaveChanges();
            return ctx;
        }

        [Fact]
        public async Task GetAllAsync_Retourne_tous_les_magasins()
        {
            // Arrange
            var ctx = CreateContextWithData();
            var logger = Mock.Of<ILogger<MagasinService>>();
            var service = new MagasinService(logger, ctx);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(m => m.Nom == "Mag1");
        }

        [Fact]
        public async Task GetMagasinByIdAsync_Id_existant_Retourne_magasin()
        {
            // Arrange
            var ctx = CreateContextWithData();
            var logger = Mock.Of<ILogger<MagasinService>>();
            var service = new MagasinService(logger, ctx);

            // Act
            var mag = await service.GetMagasinByIdAsync(1);

            // Assert
            mag.Should().NotBeNull();
            mag.MagasinId.Should().Be(1);
        }

        [Fact]
        public async Task GetMagasinByIdAsync_Id_inexistant_Retourne_null()
        {
            // Arrange
            var ctx = CreateContextWithData();
            var logger = Mock.Of<ILogger<MagasinService>>();
            var service = new MagasinService(logger, ctx);

            // Act
            var mag = await service.GetMagasinByIdAsync(99);

            // Assert
            mag.Should().BeNull();
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenServiceIsNull()
        {
            // Arrange
            var logger = Mock.Of<ILogger<MagasinService>>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MagasinService(logger, null!));
        }
    }
}
