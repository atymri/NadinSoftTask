using Moq;
using ProductManager.Core.Domain.Entities;
using ProductManager.Infrastructure.DatabaseContext;
using ProductManager.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;

namespace ProductManager.Infrastructure.Tests.Repositories
{
    public class ProductRepositoryTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ProductRepository _repository;

        public ProductRepositoryTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
            var dbContextMock = new DbContextMock<ApplicationDbContext>(dbContextOptions);

            dbContextMock.CreateDbSetMock(x => x.Products);

            _dbContext = dbContextMock.Object;
            _repository = new ProductRepository(_dbContext);
        }

        [Fact]
        public async Task GetProductByIdAsync_WhenProductExists_ReturnsProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { ID = productId, Name = "Test Product" };

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetProductByIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ID);
        }

        [Fact]
        public async Task GetProductByIdAsync_WhenProductNotExists_ReturnsNull()
        {
            // Act
            var result = await _repository.GetProductByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsAllProductsOrderedByDate()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { ID = Guid.NewGuid(), Name = "Product1", Date = DateTime.Now.AddDays(-2) },
                new Product { ID = Guid.NewGuid(), Name = "Product2", Date = DateTime.Now.AddDays(-1) },
                new Product { ID = Guid.NewGuid(), Name = "Product3", Date = DateTime.Now }
            };

            _dbContext.Products.AddRange(products);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal(products.OrderBy(p => p.Date).Select(p => p.Name), result.Select(p => p.Name));
        }

        [Fact]
        public async Task GetProductsByNameAsync_WhenNameExists_ReturnsProducts()
        {
            // Arrange
            var productName = "Test Product";
            var products = new List<Product>
            {
                new Product { ID = Guid.NewGuid(), Name = productName },
                new Product { ID = Guid.NewGuid(), Name = productName },
                new Product { ID = Guid.NewGuid(), Name = "Other Product" }
            };

            _dbContext.Products.AddRange(products);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetProductsByNameAsync(productName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.Equal(productName, p.Name));
        }

        [Fact]
        public async Task GetProductsByNameAsync_WhenNameNotExists_ReturnsNull()
        {
            // Act
            var result = await _repository.GetProductsByNameAsync("NonExisting");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProductsByNameAsync_WhenNameIsNullOrEmpty_ReturnsNull()
        {
            // Act & Assert
            var result1 = await _repository.GetProductsByNameAsync(null);
            Assert.Null(result1);

            var result2 = await _repository.GetProductsByNameAsync("");
            Assert.Null(result2);
        }

        [Fact]
        public async Task GetProductsByManufactureAsync_WhenEmailExists_ReturnsProducts()
        {
            // Arrange
            var email = "test@manufacturer.com";
            var products = new List<Product>
            {
                new Product { ID = Guid.NewGuid(), ManufactureEmail = email },
                new Product { ID = Guid.NewGuid(), ManufactureEmail = email },
                new Product { ID = Guid.NewGuid(), ManufactureEmail = "other@manufacturer.com" }
            };

            _dbContext.Products.AddRange(products);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetProductsByManufactureAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.Equal(email, p.ManufactureEmail));
        }

        [Fact]
        public async Task GetProductsByManufactureAsync_WhenEmailNotExists_ReturnsNull()
        {
            // Act
            var result = await _repository.GetProductsByManufactureAsync("nonexisting@test.com");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProductsByManufactureAsync_WhenEmailIsNullOrEmpty_ReturnsNull()
        {
            // Act & Assert
            var result1 = await _repository.GetProductsByManufactureAsync(null);
            Assert.Null(result1);

            var result2 = await _repository.GetProductsByManufactureAsync("");
            Assert.Null(result2);
        }

        [Fact]
        public async Task IsProductForManufactureAsync_WhenProductBelongsToManufacture_ReturnsTrue()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var email = "test@manufacturer.com";
            var phone = "123456789";
            var product = new Product { ID = productId, ManufactureEmail = email, ManufacturePhone = phone };

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.IsProductForManufactureAsync(email, phone, productId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsProductForManufactureAsync_WhenProductNotExists_ReturnsFalse()
        {
            // Act
            var result = await _repository.IsProductForManufactureAsync("test@test.com", "123", Guid.NewGuid());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsProductForManufactureAsync_WhenManufactureInfoNotMatch_ReturnsFalse()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { ID = productId, ManufactureEmail = "correct@test.com", ManufacturePhone = "correct123" };

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.IsProductForManufactureAsync("wrong@test.com", "wrong123", productId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddProductAsync_WhenValidProduct_AddsAndReturnsProduct()
        {
            // Arrange
            var product = new Product { ID = Guid.NewGuid(), Name = "New Product" };

            // Act
            var result = await _repository.AddProductAsync(product);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.ID, result.ID);
            Assert.Single(_dbContext.Products);
        }

        [Fact]
        public async Task AddProductsAsync_WhenValidProducts_AddsAndReturnsProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { ID = Guid.NewGuid(), Name = "Product1" },
                new Product { ID = Guid.NewGuid(), Name = "Product2" }
            };

            // Act
            var result = await _repository.AddProducstAsync(products);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(2, _dbContext.Products.Count());
        }

        [Fact]
        public async Task UpdateProductAsync_WhenProductExists_UpdatesAndReturnsProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var originalProduct = new Product { ID = productId, Name = "Original Name", ManufactureEmail = "original@test.com" };

            _dbContext.Products.Add(originalProduct);

            var updatedProduct = new Product { ID = productId, Name = "Updated Name", ManufactureEmail = "updated@test.com" };

            // Act
            var result = await _repository.UpdateProductAsync(updatedProduct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.Name);
            Assert.Equal("updated@test.com", result.ManufactureEmail);
        }

        [Fact]
        public async Task UpdateProductAsync_WhenProductNotExists_ReturnsNull()
        {
            // Arrange
            var nonExistingProduct = new Product { ID = Guid.NewGuid(), Name = "Test" };

            // Act
            var result = await _repository.UpdateProductAsync(nonExistingProduct);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteProductAsync_WhenProductExists_DeletesAndReturnsTrue()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { ID = productId, Name = "To Delete" };

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteProductAsync(productId);

            // Assert
            Assert.True(result);
            Assert.Empty(_dbContext.Products);
        }

        [Fact]
        public async Task DeleteProductAsync_WhenProductNotExists_ReturnsFalse()
        {
            // Act
            var result = await _repository.DeleteProductAsync(Guid.NewGuid());

            // Assert
            Assert.False(result);
        }
    }
}