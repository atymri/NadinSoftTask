using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductManager.Core.DTOs.ProductDTOs;

namespace ProductManager.Core.ServiceContracts
{
    public interface IProductGetterService
    {
        Task<List<ProductResponse>?> GetAllProductsAsync();
        Task<ProductResponse?> GetProductByIdAsync(Guid productId);
        Task<List<ProductResponse>?> GetProductsByNameAsync(string name);
        Task<List<ProductResponse>?> GetProductsByManufactureAsync(string manufactureEmail);
    }
}
