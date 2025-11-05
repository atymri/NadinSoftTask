using Moq;
using AutoMapper;
using ProductManager.Core.Domain.Entities;
using ProductManager.Core.Domain.RepositoryContracts;
using ProductManager.Core.DTOs.ProductDTOs;
using ProductManager.Core.Services;
using ProductManager.Core.Helpers;

namespace ProductManager.Core.Tests.Services
{
    public class ProductAdderServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductAdderService _service;

        public ProductAdderServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new ProductAdderService(_mockProductRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task AddProductAsync_WhenValidRequest_ReturnsProductResponse()
        {
            // Arrange
            var request = new ProductAddRequest
            {
                Name = "Test Product",
                ManufactureEmail = "test@gmail.com",
                ManufacturePhone = "1234567890",
                Count = 20
            };

            var product = new Product { ID = Guid.NewGuid(), Name = "Test Product", Count = 20 };
            var productResponse = new ProductResponse { ID = product.ID, Name = "Test Product", Count = 20 };

            _mockMapper.Setup(x => x.Map<Product>(request)).Returns(product);
            _mockProductRepository.Setup(x => x.AddProductAsync(product)).ReturnsAsync(product);
            _mockMapper.Setup(x => x.Map<ProductResponse>(product)).Returns(productResponse);

            // Act
            var result = await _service.AddProductAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.ID, result.ID);
            _mockMapper.Verify(x => x.Map<Product>(request), Times.Once);
            _mockProductRepository.Verify(x => x.AddProductAsync(product), Times.Once);
            _mockMapper.Verify(x => x.Map<ProductResponse>(product), Times.Once);
        }

        [Fact]
        public async Task AddProductAsync_WhenRequestIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddProductAsync(null));
        }

        [Fact]
        public async Task AddProductAsync_WhenInvalidRequest_ThrowsArgumentException()
        {
            // Arrange
            var invalidRequest = new ProductAddRequest
            {
                Name = "", // Invalid - empty name
                ManufactureEmail = "test@gmail.com",
                ManufacturePhone = "1234567890"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.AddProductAsync(invalidRequest));
        }

        [Fact]
        public async Task AddProductAsync_WhenInvalidEmail_ThrowsArgumentException()
        {
            // Arrange
            var invalidRequest = new ProductAddRequest
            {
                Name = "Test Product",
                ManufactureEmail = "invalid-email", // Invalid email
                ManufacturePhone = "1234567890"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.AddProductAsync(invalidRequest));
        }

        [Fact]
        public async Task AddProductAsync_WhenUnsupportedDomain_ThrowsArgumentException()
        {
            // Arrange
            var invalidRequest = new ProductAddRequest
            {
                Name = "Test Product",
                ManufactureEmail = "test@unsupported.com", // Unsupported domain
                ManufacturePhone = "1234567890"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.AddProductAsync(invalidRequest));
        }

        [Fact]
        public async Task AddProductsAsync_WhenValidRequests_ReturnsProductResponses()
        {
            // Arrange
            var requests = new List<ProductAddRequest>
            {
                new ProductAddRequest
                {
                    Name = "Product1",
                    ManufactureEmail = "test1@gmail.com",
                    ManufacturePhone = "1111111111",
                    Count = 20
                },
                new ProductAddRequest
                {
                    Name = "Product2",
                    ManufactureEmail = "test2@gmail.com",
                    ManufacturePhone = "2222222222",
                    Count = 20
                }
            };

            var products = new List<Product>
            {
                new Product { ID = Guid.NewGuid(), Name = "Product1" , Count = 20},
                new Product { ID = Guid.NewGuid(), Name = "Product2" , Count = 20 }
            };

            var productResponses = new List<ProductResponse>
            {
                new ProductResponse { ID = products[0].ID, Name = "Product1" , Count = 20},
                new ProductResponse { ID = products[1].ID, Name = "Product2", Count = 20 }
            };

            _mockMapper.Setup(x => x.Map<List<Product>>(requests)).Returns(products);
            _mockProductRepository.Setup(x => x.AddProducstAsync(products)).ReturnsAsync(products);
            _mockMapper.Setup(x => x.Map<List<ProductResponse>>(products)).Returns(productResponses);

            // Act
            var result = await _service.AddProductsAsync(requests);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _mockMapper.Verify(x => x.Map<List<Product>>(requests), Times.Once);
            _mockProductRepository.Verify(x => x.AddProducstAsync(products), Times.Once);
            _mockMapper.Verify(x => x.Map<List<ProductResponse>>(products), Times.Once);
        }

        [Fact]
        public async Task AddProductsAsync_WhenRequestsIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddProductsAsync(null));
        }

        [Fact]
        public async Task AddProductsAsync_WhenOneRequestInvalid_ThrowsArgumentException()
        {
            // Arrange
            var requests = new List<ProductAddRequest>
            {
                new ProductAddRequest
                {
                    Name = "Valid Product",
                    ManufactureEmail = "valid@gmail.com",
                    ManufacturePhone = "1111111111"
                },
                new ProductAddRequest
                {
                    Name = "", // Invalid - empty name
                    ManufactureEmail = "valid@gmail.com",
                    ManufacturePhone = "2222222222"
                }
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.AddProductsAsync(requests));
        }

        [Fact]
        public async Task AddProductsAsync_WhenOneEmailInvalid_ThrowsArgumentException()
        {
            // Arrange
            var requests = new List<ProductAddRequest>
            {
                new ProductAddRequest
                {
                    Name = "Product1",
                    ManufactureEmail = "valid@gmail.com",
                    ManufacturePhone = "1111111111"
                },
                new ProductAddRequest
                {
                    Name = "Product2",
                    ManufactureEmail = "invalid-email", // Invalid email
                    ManufacturePhone = "2222222222"
                }
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.AddProductsAsync(requests));
        }
    }
}