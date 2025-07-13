using AdministrationMcService.Controllers;
using AdministrationMcService.Models;
using AdministrationMcService.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AdministrationMcService.Test.UnitTests.Controllers
{
    public class MagasinControllerTests
    {
        private readonly Mock<IMagasinService> _svcMock;
        private readonly Mock<ILogger<MagasinController>> _logMock;
        private readonly MagasinController _ctrl;

        public MagasinControllerTests()
        {
            _svcMock = new Mock<IMagasinService>();
            _logMock = new Mock<ILogger<MagasinController>>();
            _ctrl = new MagasinController(_logMock.Object, _svcMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAll()
        {
            // Arrange
            var data = new List<Magasin> { new Magasin { MagasinId = 1, Nom = "M1" } };
            _svcMock.Setup(s => s.GetAllAsync()).ReturnsAsync(data);

            // Act
            var action = await _ctrl.GetAll();

            // Assert
            var ok = action.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(data);
        }

        [Fact]
        public async Task GetById_ShouldReturnMagasin_WhenExists()
        {
            // Arrange
            var m = new Magasin { MagasinId = 5, Nom = "Test" };
            _svcMock.Setup(s => s.GetMagasinByIdAsync(5)).ReturnsAsync(m);

            // Act
            var action = await _ctrl.GetById(5);

            // Assert
            var ok = action.Should().BeOfType<OkObjectResult>().Subject;
            ((Magasin)ok.Value!).MagasinId.Should().Be(5);
        }

        [Fact]
        public async Task GetById_ShouldReturn_NotFound()
        {
            // Arrange
            _svcMock.Setup(s => s.GetMagasinByIdAsync(42)).ReturnsAsync((Magasin?)null);

            // Act
            var action = await _ctrl.GetById(42);

            // Assert
            action.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenServiceIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MagasinController(_logMock.Object, null!));
        }
    }
}
