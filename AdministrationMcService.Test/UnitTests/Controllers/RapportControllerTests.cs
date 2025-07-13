using System;
using System.Threading.Tasks;
using AdministrationMcService.Controllers;
using AdministrationMcService.Models;
using AdministrationMcService.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AdministrationMcService.Test.UnitTests.Controllers
{
    public class RapportControllerTests
    {
        private readonly Mock<IRapportService> _svcMock;
        private readonly Mock<ILogger<RapportController>> _logMock;
        private readonly RapportController _ctrl;

        public RapportControllerTests()
        {
            _svcMock = new Mock<IRapportService>();
            _logMock = new Mock<ILogger<RapportController>>();
            _ctrl = new RapportController(_logMock.Object, _svcMock.Object);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenLoggerIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new RapportController(null!, _svcMock.Object));
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenServiceIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new RapportController(_logMock.Object, null!));
        }
    }
}
