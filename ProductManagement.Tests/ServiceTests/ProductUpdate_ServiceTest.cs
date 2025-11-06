using Moq;
using AutoMapper;
using ProductManager.Core.Domain.Entities;
using ProductManager.Core.Domain.RepositoryContracts;
using ProductManager.Core.DTOs.ProductDTOs;
using ProductManager.Core.Services;

namespace ProductManager.Core.Tests.Services
{
    public class ProductUpdaterServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductUpdaterService _service;

        public ProductUpdaterServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new ProductUpdaterService(_mockProductRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task UpdateProductAsync_WhenValidRequest_ReturnsProductResponse()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var request = new ProductUpdateRequest
            {
                ID = productId,
                Name = "Updated Product",
                ManufactureEmail = "updated@gmail.com",
                ManufacturePhone = "1234567890",
                Count = 20
            };

            var product = new Product { ID = productId, Count = 20, Name = "Updated Product" };
            var updatedProduct = new Product { ID = productId, Count = 20, Name = "Updated Product" };
            var productResponse = new ProductResponse { ID = productId, Count = 20, Name = "Updated Product" };

            _mockMapper.Setup(x => x.Map<Product>(request)).Returns(product);
            _mockProductRepository.Setup(x => x.UpdateProductAsync(product)).ReturnsAsync(updatedProduct);
            _mockMapper.Setup(x => x.Map<ProductResponse>(updatedProduct)).Returns(productResponse);

            // Act
            var result = await _service.UpdateProductAsync(productId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ID);
            _mockMapper.Verify(x => x.Map<Product>(request), Times.Once);
            _mockProductRepository.Verify(x => x.UpdateProductAsync(product), Times.Once);
            _mockMapper.Verify(x => x.Map<ProductResponse>(updatedProduct), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_WhenProductIdMismatch_ThrowsArgumentException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var differentProductId = Guid.NewGuid();
            var request = new ProductUpdateRequest
            {
                ID = differentProductId,
                Name = "Test Product",
                ManufactureEmail = "test@gmail.com",
                ManufacturePhone = "1234567890"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.UpdateProductAsync(productId, request));

            Assert.Contains($"آیدی {productId} برای محصول {request.Name} نیست", exception.Message);
        }

        [Fact]
        public async Task UpdateProductAsync_WhenInvalidRequest_ThrowsArgumentException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var request = new ProductUpdateRequest
            {
                ID = productId,
                Name = "", // Invalid - empty name
                ManufactureEmail = "test@gmail.com",
                ManufacturePhone = "1234567890"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateProductAsync(productId, request));
        }

        [Fact]
        public async Task UpdateProductAsync_WhenInvalidEmail_ThrowsArgumentException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var request = new ProductUpdateRequest
            {
                ID = productId,
                Name = "Test Product",
                ManufactureEmail = "invalid-email", // Invalid email
                ManufacturePhone = "1234567890"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateProductAsync(productId, request));
        }

        [Fact]
        public async Task UpdateProductAsync_WhenUnsupportedDomain_ThrowsArgumentException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var request = new ProductUpdateRequest
            {
                ID = productId,
                Name = "Test Product",
                ManufactureEmail = "test@unsupported.com", // Unsupported domain
                ManufacturePhone = "1234567890"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateProductAsync(productId, request));
        }

        [Fact]
        public async Task UpdateProductAsync_WhenProductNotExists_ReturnsNull()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var request = new ProductUpdateRequest
            {
                ID = productId,
                Name = "Test Product",
                ManufactureEmail = "test@gmail.com",
                ManufacturePhone = "1234567890",
                Count = 20
            };

            var product = new Product { ID = productId, Count = 20, Name = "Test Product" };

            _mockMapper.Setup(x => x.Map<Product>(request)).Returns(product);
            _mockProductRepository.Setup(x => x.UpdateProductAsync(product)).ReturnsAsync((Product?)null);

            // Act
            var result = await _service.UpdateProductAsync(productId, request);

            // Assert
            Assert.Null(result);
            _mockMapper.Verify(x => x.Map<Product>(request), Times.Once);
            _mockProductRepository.Verify(x => x.UpdateProductAsync(product), Times.Once);
        }
    }
}