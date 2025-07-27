using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdharManagementSystem.Controller;
using AdharManagementSystem.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AdharManagementSystem.UnitTests.Controller
{
    public class AdharControllerTests
    {
        private readonly Mock<IAdharService> _mockAdharService;
        private readonly Mock<Microsoft.Extensions.Logging.ILogger<AdharManagementSystem.Controller.AdharController>> _mockLogger;
        private readonly AdharController _controller;

        public AdharControllerTests()
        {
            _mockAdharService = new Mock<IAdharService>();
            _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<AdharManagementSystem.Controller.AdharController>>();
            _controller = new AdharController(_mockAdharService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Get_ReturnsOkWithAdharNumbers_WhenAdharsExist()
        {
            // Arrange
            var adhars = new List<Models.Adhar>
            {
                new Models.Adhar { Number = 123 },
                new Models.Adhar { Number = 456 }
            };
            _mockAdharService.Setup(s => s.GetAdhars()).ReturnsAsync(adhars);

            // Act
            var result = await _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var numbers = Assert.IsAssignableFrom<List<int>>(okResult.Value);
            Assert.Equal(2, numbers.Count);
            Assert.Contains(123, numbers);
            Assert.Contains(456, numbers);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenNoAdharsExist()
        {
            // Arrange
            _mockAdharService.Setup(s => s.GetAdhars()).ReturnsAsync(new List<Models.Adhar>());

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAccount_ReturnsNoContent_WhenCancellationRequested()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act
            var result = await _controller.GetAccount(123, cts.Token);

            // Assert
            var objectResult = Assert.IsType<Microsoft.AspNetCore.Mvc.ObjectResult>(result);
            Assert.Equal(499, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetAccount_ReturnsOkWithId_WhenAdharExists()
        {
            // Arrange
            var adhar = new Models.Adhar { Id = Guid.NewGuid(), Number = 123 };
            _mockAdharService.Setup(s => s.GetSingleAdhar(123)).ReturnsAsync(adhar);

            // Act
            var result = await _controller.GetAccount(123, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(adhar.Id, okResult.Value);
        }

        [Fact]
        public async Task GetAccount_ReturnsNotFound_WhenAdharDoesNotExist()
        {
            // Arrange
            _mockAdharService.Setup(s => s.GetSingleAdhar(123)).ReturnsAsync((Models.Adhar)null);

            // Act
            var result = await _controller.GetAccount(123, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsNoContent_WhenCancellationRequested()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act
            var result = await _controller.Create(123, cts.Token);

            // Assert
            var objectResult = Assert.IsType<Microsoft.AspNetCore.Mvc.ObjectResult>(result);
            Assert.Equal(499, objectResult.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsCreated_WhenNewAdharCreated()
        {
            // Arrange
            var newAdhar = new Models.Adhar { Id = Guid.NewGuid(), Number = 123 };
            _mockAdharService.Setup(s => s.CreateNewAdhar(123)).ReturnsAsync(newAdhar);

            // Act
            var result = await _controller.Create(123, CancellationToken.None);

            // Assert
            var createdResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            Assert.Equal(newAdhar, createdResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenCreationFails()
        {
            // Arrange
            _mockAdharService.Setup(s => s.CreateNewAdhar(123)).ReturnsAsync((Models.Adhar)null);

            // Act
            var result = await _controller.Create(123, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenCancellationRequested()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act
            var result = await _controller.Update(Guid.NewGuid(), 456, cts.Token);

            // Assert
            var objectResult = Assert.IsType<Microsoft.AspNetCore.Mvc.ObjectResult>(result);
            Assert.Equal(499, objectResult.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenUpdateSucceeds()
        {
            // Arrange
            var updatedAdhar = new Models.Adhar { Id = Guid.NewGuid(), Number = 456 };
            _mockAdharService.Setup(s => s.UpdateAdhar(updatedAdhar.Id, 456)).ReturnsAsync(updatedAdhar);

            // Act
            var result = await _controller.Update(updatedAdhar.Id, 456, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(updatedAdhar, okResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenUpdateFails()
        {
            // Arrange
            _mockAdharService.Setup(s => s.UpdateAdhar(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync((Models.Adhar)null);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), 456, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenCancellationRequested()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act
            var result = await _controller.Delete(Guid.NewGuid(), cts.Token);

            // Assert
            var objectResult = Assert.IsType<Microsoft.AspNetCore.Mvc.ObjectResult>(result);
            Assert.Equal(499, objectResult.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenDeleteSucceeds()
        {
            // Arrange
            var adharId = Guid.NewGuid();
            var deletedId = adharId;
            _mockAdharService.Setup(s => s.DeleteAdhar(adharId)).ReturnsAsync(deletedId);

            // Act
            var result = await _controller.Delete(adharId, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(deletedId, okResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenDeleteFails()
        {
            // Arrange
            _mockAdharService.Setup(s => s.DeleteAdhar(It.IsAny<Guid>())).ReturnsAsync((Guid?)null);

            // Act
            var result = await _controller.Delete(Guid.NewGuid(), CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
}
