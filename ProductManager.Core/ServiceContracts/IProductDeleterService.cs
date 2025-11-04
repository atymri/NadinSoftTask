using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductManager.Core.DTOs.ProductDTOs;

namespace ProductManager.Core.ServiceContracts
{
    public interface IProductDeleterService
    {
        Task<bool> DeleteProductAsync(Guid productId);
        Task<bool> DeleteProductsAsync(List<ProductResponse?> products);
        Task<bool> DeleteProductsBeforeThan(DateTime date);
    }
}
