using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdministrationMcService.Data;
using AdministrationMcService.Models;
using AdministrationMcService.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AdministrationMcService.Test.UnitTests.Services
{
    public class PerformanceServiceTests
    {
        private AdminDbContext CreateContext(IEnumerable<Performance> seed = null)
        {
            var options = new DbContextOptionsBuilder<AdminDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var ctx = new AdminDbContext(options);
            if (seed != null)
            {
                ctx.Performances.AddRange(seed);
                ctx.SaveChanges();
            }
            return ctx;
        }

        [Fact]
        public async Task GetAllPerformancesAsync_Retourne_toutes_les_performances()
        {
            // Arrange
            var performancesSeed = new[]
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
                },
                new Performance
                {
                    PerformanceId = 2,
                    MagasinId = 3,
                    Date = DateTime.UtcNow.Date.AddDays(-2),
                    ChiffreAffaires = 1500m,
                    NbVentes = 70,
                    RupturesStock = 1,
                    Surstock = 3
                }
            };
            var ctx = CreateContext(performancesSeed);
            var logger = Mock.Of<ILogger<PerformanceService>>();
            var svc = new PerformanceService(logger, ctx);

            // Act
            var result = await svc.GetAllPerformancesAsync();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public void Ctor_NullContext_Lance_ArgumentNullException()
        {
            // Arrange
            var logger = Mock.Of<ILogger<PerformanceService>>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => new PerformanceService(logger, null!));
        }

        [Fact]
        public void Ctor_NullLogger_Lance_ArgumentNullException()
        {
            // Arrange
            var ctx = CreateContext();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => new PerformanceService(null!, ctx));
        }
    }
}
