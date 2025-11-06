using AutoMapper;
using ProductManager.Core.Domain.Entities;
using ProductManager.Core.Domain.RepositoryContracts;
using ProductManager.Core.DTOs.ProductDTOs;
using ProductManager.Core.Helpers;
using ProductManager.Core.ServiceContracts;

namespace ProductManager.Core.Services
{
    public class ProductUpdaterService : IProductUpdaterService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public ProductUpdaterService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ProductResponse?> UpdateProductAsync(Guid productId, ProductUpdateRequest request)
        {
            if (productId != request.ID)
                throw new ArgumentException($"آیدی {productId} برای محصول {request.Name} نیست");

            ValidationHelper.Validate(request);
            ValidationHelper.ValidateEmail(request.ManufactureEmail);

            var product = _mapper.Map<Product>(request);
            var updated = await _productRepository.UpdateProductAsync(product);
            var res = _mapper.Map<ProductResponse>(updated);

            return res ?? null;
        }
    }
}
