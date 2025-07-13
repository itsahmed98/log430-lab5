using AdministrationMcService.Controllers;
using AdministrationMcService.Models;
using AdministrationMcService.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AdministrationMcService.Test.UnitTests.Controllers
{
    public class PerformanceControllerTests
    {
        private readonly Mock<IPerformanceService> _svcMock;
        private readonly Mock<ILogger<PerformanceController>> _logMock;
        private readonly PerformanceController _ctrl;

        public PerformanceControllerTests()
        {
            _svcMock = new Mock<IPerformanceService>();
            _logMock = new Mock<ILogger<PerformanceController>>();
            _ctrl = new PerformanceController(_logMock.Object, _svcMock.Object);
        }

        [Fact]
        public async Task GetAll_OK_et_retourne_liste()
        {
            // Arrange
            var data = new List<Performance>
            {
                new Performance
                {
                    PerformanceId = 1,
                    MagasinId = 2,
                    Date = DateTime.UtcNow.Date.AddDays(-2),
                    ChiffreAffaires = 1000m,
                    NbVentes = 50,
                    RupturesStock = 2,
                    Surstock = 5
                }
            };
            _svcMock.Setup(s => s.GetAllPerformancesAsync())
                    .ReturnsAsync(data);

            // Act
            var action = await _ctrl.GetAll();

            // Assert
            var ok = action.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(data);
        }

        [Fact]
        public void Ctor_NullService_Lance_ArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => new PerformanceController(_logMock.Object, null!));
        }

        [Fact]
        public void Ctor_NullLogger_Lance_ArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => new PerformanceController(null!, _svcMock.Object));
        }
    }
}
