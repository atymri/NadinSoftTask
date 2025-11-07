using AutoMapper;
using ProductManager.Core.Domain.RepositoryContracts;
using ProductManager.Core.DTOs.ProductDTOs;
using ProductManager.Core.Helpers;
using ProductManager.Core.ServiceContracts;

namespace ProductManager.Core.Services
{
    public class ProductGetterService : IProductGetterService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public ProductGetterService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<List<ProductResponse>?> GetAllProductsAsync()
            => _mapper.Map<List<ProductResponse>>(await _productRepository.GetAllProductsAsync())
            ?? null;


        public async Task<ProductResponse?> GetProductByIdAsync(Guid productId)
            => _mapper.Map<ProductResponse>(await _productRepository.GetProductByIdAsync(productId))
            ?? null;


        public async Task<List<ProductResponse>?> GetProductsByManufactureAsync(string manufactureEmail)
        {
            ValidationHelper.ValidateEmail(manufactureEmail);
            return _mapper.Map<List<ProductResponse>>(
                    await _productRepository.GetProductsByManufactureAsync(manufactureEmail))
                ?? null;
        }

        public async Task<List<ProductResponse>?> GetProductsByNameAsync(string name)
            => _mapper.Map<List<ProductResponse>>(await _productRepository.GetProductsByNameAsync(name))
            ?? null;
    }
}
