using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductManager.Core.Domain.Entities;

namespace ProductManager.Core.Domain.RepositoryContracts
{
    public interface IProductRepository
    {
        Task<List<Product>?> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(Guid productId);
        Task<List<Product>?> GetProductsByNameAsync(string name);
        Task<List<Product>?> GetProductsByManufactureAsync(string? manufactureEmail);
        Task<bool> IsProductForManufactureAsync(string manufactureEmail, string manufacturePhone, Guid productId); // for delete, update and add.
        Task<Product?> AddProductAsync(Product product);
        Task<Product?> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(Guid productId);
    }
}
