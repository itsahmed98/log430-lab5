using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenteMcService.Controllers;
using VenteMcService.Models;
using VenteMcService.Services;

namespace VenteMcService.Test.IntegrationTests
{
    public class VenteControllerTests
    {
        private readonly Mock<IVenteService> _mockService = new();
        private readonly Mock<Microsoft.Extensions.Logging.ILogger<VenteController>> _mockLogger = new();

        private VenteController CreateController()
            => new VenteController(_mockLogger.Object, _mockService.Object);

        [Fact]
        public async Task GetAll_Returns_Ok_With_List()
        {
            _mockService.Setup(s => s.GetAllAsync())
                        .ReturnsAsync(new List<Vente> { new Vente { VenteId = 1 } });

            var ctrl = CreateController();
            var result = await ctrl.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<Vente>>(ok.Value);
            Assert.Single(list);
        }

        [Fact]
        public async Task GetById_Returns_NotFound_When_Null()
        {
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Vente?)null);

            var ctrl = CreateController();
            var result = await ctrl.GetById(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_When_ModelState_Invalid()
        {
            var ctrl = CreateController();
            ctrl.ModelState.AddModelError("Date", "Required");

            var result = await ctrl.Create(new VenteDto());
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Delete_Returns_NoContent_On_Success()
        {
            _mockService.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);

            var ctrl = CreateController();
            var result = await ctrl.Delete(1);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
