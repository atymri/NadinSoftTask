using ProductManager.Core.DTOs.ProductDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManager.Core.ServiceContracts
{
    public interface IProductUpdaterService
    {
        Task<ProductResponse?> UpdateProductAsync(Guid productId, ProductUpdateRequest request);
    }
}
