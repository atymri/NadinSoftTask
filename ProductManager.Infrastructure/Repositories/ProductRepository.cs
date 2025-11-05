using Microsoft.EntityFrameworkCore;
using ProductManager.Core.Domain.Entities;
using ProductManager.Core.Domain.RepositoryContracts;
using ProductManager.Infrastructure.DatabaseContext;

namespace ProductManager.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>?> GetAllProductsAsync()
             => await _context.Products
                 .OrderBy(p => p.Date)
                 .ToListAsync();

        public async Task<Product?> GetProductByIdAsync(Guid productId)
             => await _context.Products.FindAsync(productId);

        public async Task<List<Product>?> GetProductsByNameAsync(string name)
            => !string.IsNullOrEmpty(name)
                ? await _context.Products.Where(p => p.Name == name.Trim()).ToListAsync()
                : null;

        public async Task<List<Product>?> GetProductsByManufactureAsync(string? manufactureEmail)
            => !string.IsNullOrEmpty(manufactureEmail) 
                ? await _context.Products.Where(p => p.ManufactureEmail == manufactureEmail.Trim()).ToListAsync()
                :  null;

        public async Task<bool> IsProductForManufactureAsync(string manufactureEmail, string manufacturePhone, Guid productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            return product.ManufactureEmail == manufactureEmail.Trim()
                   && product.ManufacturePhone == manufacturePhone.Trim();
        }

        public async Task<Product?> AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return await GetProductByIdAsync(product.ID);
        }

        public async Task<List<Product>?> AddProducstAsync(List<Product> products)
        {
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            return products;
        }
        public async Task<Product?> UpdateProductAsync(Product product)
        {
            var existingProduct = await _context.Products.FindAsync(product.ID);
            if (existingProduct == null) return null;

            _context.Entry(existingProduct).CurrentValues.SetValues(product);

            await _context.SaveChangesAsync();
            return existingProduct;
        }
        public async Task<bool> DeleteProductAsync(Guid productId)
        {
            var existingProduct = await _context.Products.FindAsync(productId);
            if (existingProduct == null) return false;

            _context.Products.Remove(existingProduct);
            var rowsDeleted = await _context.SaveChangesAsync();
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
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
