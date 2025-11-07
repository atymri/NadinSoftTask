using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ProductManager.Core.Domain.Entities;
using ProductManager.Core.Domain.RepositoryContracts;
using ProductManager.Infrastructure.DatabaseContext;

namespace ProductManager.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;   // SQL Server
        private readonly MongoDbContext _mongoContext;    // MongoDB

        public ProductRepository(ApplicationDbContext context, MongoDbContext mongoContext)
        {
            _context = context;
            _mongoContext = mongoContext;
        }

        // ================= Read (MongoDB) =================

        public async Task<List<Product>?> GetAllProductsAsync()
        {
            return await _mongoContext.Products.Find(_ => true)
                .SortBy(p => p.Date)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(Guid productId)
        {
            return await _mongoContext.Products
                .Find(p => p.ID == productId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Product>?> GetProductsByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            return await _mongoContext.Products
                .Find(p => p.Name == name.Trim())
                .ToListAsync();
        }

        public async Task<List<Product>?> GetProductsByManufactureAsync(string? manufactureEmail)
        {
            if (string.IsNullOrEmpty(manufactureEmail)) return null;

            return await _mongoContext.Products
                .Find(p => p.ManufactureEmail == manufactureEmail.Trim())
                .ToListAsync();
        }

        // ================= Validation =================

        public async Task<bool> IsProductForManufactureAsync(string manufactureEmail, string manufacturePhone, Guid productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            return product.ManufactureEmail == manufactureEmail.Trim() &&
                   product.ManufacturePhone == manufacturePhone.Trim();
        }

        // ================= Write (SQL Server) =================

        public async Task<Product?> AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            await _mongoContext.Products.InsertOneAsync(product);

            return product;
        }

        public async Task<List<Product>?> AddProducstAsync(List<Product> products)
        {
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            if (products.Any())
                await _mongoContext.Products.InsertManyAsync(products);

            return products;
        }

        public async Task<Product?> UpdateProductAsync(Product product)
        {
            var existingProduct = await _context.Products.FindAsync(product.ID);
            if (existingProduct == null) return null;

            _context.Entry(existingProduct).CurrentValues.SetValues(product);
            await _context.SaveChangesAsync();

            await _mongoContext.Products.ReplaceOneAsync(p => p.ID == product.ID, product);

            return existingProduct;
        }

        public async Task<bool> DeleteProductAsync(Guid productId)
        {
            var existingProduct = await _context.Products.FindAsync(productId);
            if (existingProduct == null) return false;

            _context.Products.Remove(existingProduct);
            var rowsDeleted = await _context.SaveChangesAsync();

            await _mongoContext.Products.DeleteOneAsync(p => p.ID == productId);

            return rowsDeleted > 0;
        }

        public async Task<bool> DeleteProductsAsync(List<Product> products)
        {
            if (products == null || !products.Any()) return false;

            var productIds = products.Select(p => p.ID).ToList();
            var existingProducts = await _context.Products
                .Where(p => productIds.Contains(p.ID))
                .ToListAsync();

            if (!existingProducts.Any()) return false;

            _context.Products.RemoveRange(existingProducts);
            var result = await _context.SaveChangesAsync() > 0;

            await _mongoContext.Products.DeleteManyAsync(p => productIds.Contains(p.ID));

            return result;
        }
    }
}
