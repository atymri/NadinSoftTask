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
