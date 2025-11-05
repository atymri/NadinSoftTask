using ProductManager.Core.Domain.Entities;
using ProductManager.Core.Domain.RepositoryContracts;
using ProductManager.Core.DTOs.ProductDTOs;
using ProductManager.Core.ServiceContracts;
using AutoMapper;

namespace ProductManager.Core.Services
{
    public class ProductDeleterService : IProductDeleterService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductDeleterService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<bool> DeleteProductAsync(Guid productId)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("آیدی محصول نمی‌تواند خالی باشد", nameof(productId));

            var result = await _productRepository.DeleteProductAsync(productId);

            if (!result)
                throw new KeyNotFoundException($"محصول با آیدی {productId} یافت نشد");

            return true;
        }

        public async Task<bool> DeleteProductsAsync(List<ProductResponse> products)
        {
            if (products == null)
                throw new ArgumentNullException(nameof(products), "لیست محصولات نمی‌تواند خالی باشد");

            if (!products.Any())
                throw new ArgumentException("لیست محصولات نمی‌تواند خالی باشد", nameof(products));

            var validProducts = products.Where(p => p != null).ToList();

            if (!validProducts.Any())
                throw new ArgumentException("هیچ محصول معتبری برای حذف وجود ندارد", nameof(products));

            var entities = _mapper.Map<List<Product>>(validProducts);
            var result = await _productRepository.DeleteProductsAsync(entities);

            return result;
        }

        public async Task<bool> DeleteProductsBeforeThan(DateTime date)
        {
            if (date == DateTime.MinValue || date == DateTime.MaxValue)
                throw new ArgumentException("تاریخ وارد شده معتبر نیست", nameof(date));

            var allProducts = await _productRepository.GetAllProductsAsync();

            if (allProducts == null || !allProducts.Any())
                return false;

            var productsToDelete = allProducts.Where(p => p.Date < date).ToList();

            if (!productsToDelete.Any())
                return false;

            var result = await _productRepository.DeleteProductsAsync(productsToDelete);
            return result;
        }
    }
}