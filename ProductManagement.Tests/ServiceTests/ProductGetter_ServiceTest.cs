using Moq;
using AutoMapper;
using ProductManager.Core.Domain.Entities;
using ProductManager.Core.Domain.RepositoryContracts;
using ProductManager.Core.DTOs.ProductDTOs;
using ProductManager.Core.Services;

namespace ProductManager.Core.Tests.Services
{
    public class ProductGetterServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductGetterService _service;

        public ProductGetterServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new ProductGetterService(_mockProductRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllProductsAsync_WhenProductsExist_ReturnsProductResponses()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { ID = Guid.NewGuid(), Name = "Product1" },
                new Product { ID = Guid.NewGuid(), Name = "Product2" }
            };

            var productResponses = new List<ProductResponse>
            {
                new ProductResponse { ID = products[0].ID, Name = "Product1" },
                new ProductResponse { ID = products[1].ID, Name = "Product2" }
            };

            _mockProductRepository.Setup(x => x.GetAllProductsAsync())
                .ReturnsAsync(products);
            _mockMapper.Setup(x => x.Map<List<ProductResponse>>(products))
                .Returns(productResponses);

            // Act
            var result = await _service.GetAllProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(productResponses[0].ID, result[0].ID);
            Assert.Equal(productResponses[1].ID, result[1].ID);
        }

        [Fact]
        public async Task GetAllProductsAsync_WhenNoProducts_ReturnsNull()
        {
            // Arrange
            _mockProductRepository.Setup(x => x.GetAllProductsAsync())
                .ReturnsAsync((List<Product>?)null);
            _mockMapper.Setup(x => x.Map<List<ProductResponse>>(null))
                .Returns((List<ProductResponse>?)null);

            // Act
            var result = await _service.GetAllProductsAsync();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductByIdAsync_WhenProductExists_ReturnsProductResponse()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { ID = productId, Name = "Test Product" };
            var productResponse = new ProductResponse { ID = productId, Name = "Test Product" };

            _mockProductRepository.Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync(product);
            _mockMapper.Setup(x => x.Map<ProductResponse>(product))
                .Returns(productResponse);

            // Act
            var result = await _service.GetProductByIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ID);
            Assert.Equal("Test Product", result.Name);
        }

        [Fact]
        public async Task GetProductByIdAsync_WhenProductNotExists_ReturnsNull()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockProductRepository.Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync((Product?)null);
            _mockMapper.Setup(x => x.Map<ProductResponse>(null))
                .Returns((ProductResponse?)null);

            // Act
            var result = await _service.GetProductByIdAsync(productId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductsByNameAsync_WhenProductsExist_ReturnsProductResponses()
        {
            // Arrange
            var productName = "Test Product";
            var products = new List<Product>
            {
                new Product { ID = Guid.NewGuid(), Name = productName },
                new Product { ID = Guid.NewGuid(), Name = productName }
            };

            var productResponses = new List<ProductResponse>
            {
                new ProductResponse { ID = products[0].ID, Name = productName },
                new ProductResponse { ID = products[1].ID, Name = productName }
            };

            _mockProductRepository.Setup(x => x.GetProductsByNameAsync(productName))
                .ReturnsAsync(products);
            _mockMapper.Setup(x => x.Map<List<ProductResponse>>(products))
                .Returns(productResponses);

            // Act
            var result = await _service.GetProductsByNameAsync(productName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.Equal(productName, p.Name));
        }

        [Fact]
        public async Task GetProductsByNameAsync_WhenNoProducts_ReturnsNull()
        {
            // Arrange
            var productName = "NonExisting";
            _mockProductRepository.Setup(x => x.GetProductsByNameAsync(productName))
                .ReturnsAsync((List<Product>?)null);
            _mockMapper.Setup(x => x.Map<List<ProductResponse>>(null))
                .Returns((List<ProductResponse>?)null);

            // Act
            var result = await _service.GetProductsByNameAsync(productName);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductsByManufactureAsync_WhenProductsExist_ReturnsProductResponses()
        {
            // Arrange
            var manufactureEmail = "test@manufacturer.com";
            var products = new List<Product>
            {
                new Product { ID = Guid.NewGuid(), ManufactureEmail = manufactureEmail },
                new Product { ID = Guid.NewGuid(), ManufactureEmail = manufactureEmail }
            };

            var productResponses = new List<ProductResponse>
            {
                new ProductResponse { ID = products[0].ID, ManufactureEmail = manufactureEmail },
                new ProductResponse { ID = products[1].ID, ManufactureEmail = manufactureEmail }
            };

            _mockProductRepository.Setup(x => x.GetProductsByManufactureAsync(manufactureEmail))
                .ReturnsAsync(products);
            _mockMapper.Setup(x => x.Map<List<ProductResponse>>(products))
                .Returns(productResponses);

            // Act
            var result = await _service.GetProductsByManufactureAsync(manufactureEmail);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.Equal(manufactureEmail, p.ManufactureEmail));
        }

        [Fact]
        public async Task GetProductsByManufactureAsync_WhenNoProducts_ReturnsNull()
        {
            // Arrange
            var manufactureEmail = "nonexisting@test.com";
            _mockProductRepository.Setup(x => x.GetProductsByManufactureAsync(manufactureEmail))
                .ReturnsAsync((List<Product>?)null);
            _mockMapper.Setup(x => x.Map<List<ProductResponse>>(null))
                .Returns((List<ProductResponse>?)null);

            // Act
            var result = await _service.GetProductsByManufactureAsync(manufactureEmail);

            // Assert
            Assert.Null(result);
        }
    }
}