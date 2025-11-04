using ProductManager.Core.DTOs.ProductDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManager.Core.ServiceContracts
{
    public interface IProductAdderService
    {
        Task<ProductResponse?> AddProductAsync(ProductAddRequest request);
        Task<List<ProductResponse>?> AddProductsAsync(List<ProductAddRequest> requests);
    }
}
