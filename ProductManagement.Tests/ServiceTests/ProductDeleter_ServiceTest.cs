using Moq;
using AutoMapper;
using ProductManager.Core.Domain.Entities;
using ProductManager.Core.Domain.RepositoryContracts;
using ProductManager.Core.DTOs.ProductDTOs;
using ProductManager.Core.Services;

namespace ProductManager.Core.Tests.Services
{
    public class ProductDeleterServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductDeleterService _service;

        public ProductDeleterServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new ProductDeleterService(_mockProductRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task DeleteProductAsync_WhenProductExists_ReturnsTrue()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockProductRepository.Setup(x => x.DeleteProductAsync(productId))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteProductAsync(productId);

            // Assert
            Assert.True(result);
            _mockProductRepository.Verify(x => x.DeleteProductAsync(productId), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_WhenProductNotExists_ThrowsKeyNotFoundException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockProductRepository.Setup(x => x.DeleteProductAsync(productId))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.DeleteProductAsync(productId));

            Assert.Contains($"محصول با آیدی {productId} یافت نشد", exception.Message);
        }

        [Fact]
        public async Task DeleteProductAsync_WhenProductIdIsEmpty_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.DeleteProductAsync(Guid.Empty));

            Assert.Contains("آیدی محصول نمی‌تواند خالی باشد", exception.Message);
        }

        [Fact]
        public async Task DeleteProductsAsync_WhenValidProducts_ReturnsTrue()
        {
            // Arrange
            var products = new List<ProductResponse>
            {
                new ProductResponse { ID = Guid.NewGuid(), Name = "Product1" },
                new ProductResponse { ID = Guid.NewGuid(), Name = "Product2" }
            };

            var entities = new List<Product>
            {
                new Product { ID = products[0].ID, Name = "Product1" },
                new Product { ID = products[1].ID, Name = "Product2" }
            };

            _mockMapper.Setup(x => x.Map<List<Product>>(products))
                .Returns(entities);
            _mockProductRepository.Setup(x => x.DeleteProductsAsync(entities))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteProductsAsync(products);

            // Assert
            Assert.True(result);
            _mockMapper.Verify(x => x.Map<List<Product>>(products), Times.Once);
            _mockProductRepository.Verify(x => x.DeleteProductsAsync(entities), Times.Once);
        }

        [Fact]
        public async Task DeleteProductsAsync_WhenProductsIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.DeleteProductsAsync(null));
        }

        [Fact]
        public async Task DeleteProductsAsync_WhenProductsIsEmpty_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.DeleteProductsAsync(new List<ProductResponse>()));

            Assert.Contains("لیست محصولات نمی‌تواند خالی باشد", exception.Message);
        }

        [Fact]
        public async Task DeleteProductsAsync_WhenAllProductsAreNull_ThrowsArgumentException()
        {
            // Arrange
            var products = new List<ProductResponse> { null, null };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.DeleteProductsAsync(products));

            Assert.Contains("هیچ محصول معتبری برای حذف وجود ندارد", exception.Message);
        }

        [Fact]
        public async Task DeleteProductsBeforeThan_WhenProductsExistBeforeDate_ReturnsTrue()
        {
            // Arrange
            var date = DateTime.Now;
            var products = new List<Product>
            {
                new Product { ID = Guid.NewGuid(), Name = "Product1", Date = date.AddDays(-2) },
                new Product { ID = Guid.NewGuid(), Name = "Product2", Date = date.AddDays(-1) },
                new Product { ID = Guid.NewGuid(), Name = "Product3", Date = date.AddDays(1) }
            };

            var productsToDelete = products.Where(p => p.Date < date).ToList();

            _mockProductRepository.Setup(x => x.GetAllProductsAsync())
                .ReturnsAsync(products);
            _mockProductRepository.Setup(x => x.DeleteProductsAsync(productsToDelete))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteProductsBeforeThan(date);

            // Assert
            Assert.True(result);
            _mockProductRepository.Verify(x => x.GetAllProductsAsync(), Times.Once);
            _mockProductRepository.Verify(x => x.DeleteProductsAsync(productsToDelete), Times.Once);
        }

        [Fact]
        public async Task DeleteProductsBeforeThan_WhenNoProducts_ReturnsFalse()
        {
            // Arrange
            var date = DateTime.Now;
            _mockProductRepository.Setup(x => x.GetAllProductsAsync())
                .ReturnsAsync(new List<Product>());

            // Act
            var result = await _service.DeleteProductsBeforeThan(date);

            // Assert
            Assert.False(result);
            _mockProductRepository.Verify(x => x.GetAllProductsAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteProductsBeforeThan_WhenNoProductsBeforeDate_ReturnsFalse()
        {
            // Arrange
            var date = DateTime.Now.AddDays(-10); // Past date
            var products = new List<Product>
            {
                new Product { ID = Guid.NewGuid(), Name = "Product1", Date = DateTime.Now },
                new Product { ID = Guid.NewGuid(), Name = "Product2", Date = DateTime.Now }
            };

            _mockProductRepository.Setup(x => x.GetAllProductsAsync())
                .ReturnsAsync(products);

            // Act
            var result = await _service.DeleteProductsBeforeThan(date);

            // Assert
            Assert.False(result);
            _mockProductRepository.Verify(x => x.GetAllProductsAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteProductsBeforeThan_WhenInvalidDate_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.DeleteProductsBeforeThan(DateTime.MinValue));

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.DeleteProductsBeforeThan(DateTime.MaxValue));
        }
    }
}