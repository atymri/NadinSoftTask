using Moq;
using MongoDB.Driver;
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
        private readonly Mock<IMongoCollection<Product>> _mongoCollectionMock;
        private readonly Mock<MongoDbContext> _mongoContextMock;
        private readonly ProductRepository _repository;

        public ProductRepositoryTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
            var dbContextMock = new DbContextMock<ApplicationDbContext>(dbContextOptions);
            dbContextMock.CreateDbSetMock(x => x.Products);
            _dbContext = dbContextMock.Object;

            _mongoCollectionMock = new Mock<IMongoCollection<Product>>();
            _mongoContextMock = new Mock<MongoDbContext>("mongodb://localhost:27017", "ProductDb");
            _mongoContextMock.Setup(c => c.Products).Returns(_mongoCollectionMock.Object);

            _repository = new ProductRepository(_dbContext, _mongoContextMock.Object);
        }

        // ================== Write Tests (SQL Server) ==================

        [Fact]
        public async Task AddProductAsync_ShouldCallSqlAddAndMongoInsert()
        {
            var product = new Product { ID = Guid.NewGuid(), Name = "Test Product" };

            await _repository.AddProductAsync(product);

            // SQL Server Add
            Assert.Contains(_dbContext.Products, p => p.ID == product.ID);

            // Mongo Insert called
            _mongoCollectionMock.Verify(c => c.InsertOneAsync(
                It.Is<Product>(p => p.ID == product.ID),
                null,
                default), Times.Once);
        }

        [Fact]
        public async Task AddProducstAsync_ShouldCallSqlAddRangeAndMongoInsertMany()
        {
            var products = new List<Product>
            {
                new Product { ID = Guid.NewGuid(), Name = "P1" },
                new Product { ID = Guid.NewGuid(), Name = "P2" }
            };

            await _repository.AddProducstAsync(products);

            // SQL Server AddRange
            foreach (var p in products)
                Assert.Contains(_dbContext.Products, x => x.ID == p.ID);

            // Mongo InsertMany
            _mongoCollectionMock.Verify(c => c.InsertManyAsync(
                It.Is<List<Product>>(list => list.Count == 2),
                null,
                default), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldUpdateSqlAndMongo()
        {
            var product = new Product { ID = Guid.NewGuid(), Name = "Original" };
            _dbContext.Products.Add(product);

            var updated = new Product { ID = product.ID, Name = "Updated" };

            var result = await _repository.UpdateProductAsync(updated);

            Assert.Equal("Updated", result.Name);

            _mongoCollectionMock.Verify(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.Is<Product>(p => p.ID == product.ID && p.Name == "Updated"),
                It.IsAny<ReplaceOptions>(),
                default), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldRemoveFromSqlAndMongo()
        {
            var product = new Product { ID = Guid.NewGuid(), Name = "DeleteMe" };
            _dbContext.Products.Add(product);

            var result = await _repository.DeleteProductAsync(product.ID);

            Assert.True(result);
            Assert.DoesNotContain(_dbContext.Products, p => p.ID == product.ID);

            _mongoCollectionMock.Verify(c => c.DeleteOneAsync(
                It.IsAny<FilterDefinition<Product>>(),
                default), Times.Once);
        }

        // ================== Read Tests (MongoDB) ==================

        private void SetupMongoFind(List<Product> data)
        {
            var cursorMock = new Mock<IAsyncCursor<Product>>();
            cursorMock.Setup(_ => _.Current).Returns(data);
            cursorMock
                .SetupSequence(_ => _.MoveNext(It.IsAny<System.Threading.CancellationToken>()))
                .Returns(true)
                .Returns(false);
            cursorMock
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mongoCollectionMock.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<FindOptions<Product, Product>>(),
                default))
                .ReturnsAsync(cursorMock.Object);
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnAllProductsFromMongo()
        {
            var products = new List<Product>
            {
                new Product { ID = Guid.NewGuid(), Name = "P1", Date = DateTime.Now.AddDays(-1) },
                new Product { ID = Guid.NewGuid(), Name = "P2", Date = DateTime.Now }
            };
            SetupMongoFind(products);

            var result = await _repository.GetAllProductsAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProductFromMongo()
        {
            var product = new Product { ID = Guid.NewGuid(), Name = "P1" };
            SetupMongoFind(new List<Product> { product });

            var result = await _repository.GetProductByIdAsync(product.ID);

            Assert.NotNull(result);
            Assert.Equal(product.ID, result.ID);
        }

        [Fact]
        public async Task GetProductsByNameAsync_ShouldReturnProductsFromMongo()
        {
            var product = new Product { ID = Guid.NewGuid(), Name = "Test" };
            SetupMongoFind(new List<Product> { product });

            var result = await _repository.GetProductsByNameAsync("Test");

            Assert.Single(result);
            Assert.Equal("Test", result[0].Name);
        }

        [Fact]
        public async Task GetProductsByManufactureAsync_ShouldReturnProductsFromMongo()
        {
            var product = new Product { ID = Guid.NewGuid(), ManufactureEmail = "a@test.com" };
            SetupMongoFind(new List<Product> { product });

            var result = await _repository.GetProductsByManufactureAsync("a@test.com");

            Assert.Single(result);
            Assert.Equal("a@test.com", result[0].ManufactureEmail);
        }
    }
}
