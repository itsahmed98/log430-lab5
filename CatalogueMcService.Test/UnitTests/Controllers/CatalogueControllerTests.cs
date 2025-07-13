using CatalogueMcService.Controllers;
using CatalogueMcService.Models;
using CatalogueMcService.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CatalogueMcService.Test.UnitTests.Controllers
{
    public class CatalogueControllerTests
    {
        private readonly Mock<ICatalogueService> _svcMock;
        private readonly Mock<ILogger<CatalogueController>> _logMock;
        private readonly CatalogueController _ctrl;

        public CatalogueControllerTests()
        {
            _svcMock = new Mock<ICatalogueService>();
            _logMock = new Mock<ILogger<CatalogueController>>();
            _ctrl = new CatalogueController(_logMock.Object, _svcMock.Object);
        }

        [Fact]
        public async Task GetProduits_ShouldReturnOkWithList_WhenServiceReturnsData()
        {
            // Arrange
            var list = new List<Produit> { new Produit { ProduitId = 1 } };
            _svcMock.Setup(s => s.GetAllProduitsAsync()).ReturnsAsync(list);

            // Act
            var action = await _ctrl.GetProduits();

            // Assert
            var ok = action.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(list);
        }

        [Fact]
        public async Task GetProduits_ShouldReturnInternalServerError_WhenServiceThrows()
        {
            // Arrange
            _svcMock.Setup(s => s.GetAllProduitsAsync()).ThrowsAsync(new Exception("Boom"));

            // Act
            var action = await _ctrl.GetProduits();

            // Assert
            var result = action.Should().BeOfType<ObjectResult>().Subject;
            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task GetProduit_ShouldReturnOkWithProduit_WhenExists()
        {
            // Arrange
            var p = new Produit { ProduitId = 2 };
            _svcMock.Setup(s => s.GetProduitByIdAsync(2)).ReturnsAsync(p);

            // Act
            var action = await _ctrl.GetProduit(2);

            // Assert
            var ok = action.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(p);
        }

        [Fact]
        public async Task GetProduit_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            _svcMock.Setup(s => s.GetProduitByIdAsync(99)).ReturnsAsync((Produit?)null);

            // Act
            var action = await _ctrl.GetProduit(99);

            // Assert
            action.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetProduit_ShouldReturnInternalServerError_WhenServiceThrows()
        {
            // Arrange
            _svcMock.Setup(s => s.GetProduitByIdAsync(5)).ThrowsAsync(new Exception("Fail"));

            // Act
            var action = await _ctrl.GetProduit(5);

            // Assert
            var result = action.Should().BeOfType<ObjectResult>().Subject;
            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task ModifierProduit_ShouldReturnOkWithDto_WhenSuccess()
        {
            // Arrange
            var dto = new ProduitDto { Nom = "" };
            _svcMock.Setup(s => s.GetProduitByIdAsync(3)).ReturnsAsync(new Produit { ProduitId = 3 });
            _svcMock.Setup(s => s.ModifierProduitAsync(3, dto)).Returns(Task.CompletedTask);

            // Act
            var action = await _ctrl.ModifierProduit(3, dto);

            // Assert
            var ok = action.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeSameAs(dto);
        }

        [Fact]
        public async Task ModifierProduit_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            var dto = new ProduitDto { Nom = "" };
            _svcMock.Setup(s => s.GetProduitByIdAsync(4)).ReturnsAsync((Produit?)null);

            // Act
            var action = await _ctrl.ModifierProduit(4, dto);

            // Assert
            action.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task ModifierProduit_ShouldReturnInternalServerError_WhenServiceThrows()
        {
            // Arrange
            var dto = new ProduitDto { Nom = "" };
            _svcMock.Setup(s => s.GetProduitByIdAsync(5)).ReturnsAsync(new Produit { ProduitId = 5 });
            _svcMock.Setup(s => s.ModifierProduitAsync(5, dto)).ThrowsAsync(new Exception("Err"));

            // Act
            var action = await _ctrl.ModifierProduit(5, dto);

            // Assert
            var result = action.Should().BeOfType<ObjectResult>().Subject;
            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task Rechercher_ShouldReturnOkWithList_WhenServiceReturnsData()
        {
            // Arrange
            var data = new List<Produit> { new Produit { ProduitId = 6 } };
            _svcMock.Setup(s => s.RechercherProduitsAsync("x")).ReturnsAsync(data);

            // Act
            var action = await _ctrl.Rechercher("x");

            // Assert
            var ok = action.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(data);
        }
    }
}
