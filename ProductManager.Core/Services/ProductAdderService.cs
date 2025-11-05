using AutoMapper;
using ProductManager.Core.Domain.Entities;
using ProductManager.Core.Domain.RepositoryContracts;
using ProductManager.Core.DTOs.ProductDTOs;
using ProductManager.Core.Helpers;
using ProductManager.Core.ServiceContracts;

namespace ProductManager.Core.Services
{
    public class ProductAdderService : IProductAdderService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductAdderService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ProductResponse?> AddProductAsync(ProductAddRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            ValidationHelper.Validate(request);
            ValidationHelper.ValidateEmail(request.ManufactureEmail);
            

            var product = _mapper.Map<Product>(request);
            var response = await _productRepository.AddProductAsync(product);

            return _mapper.Map<ProductResponse>(response);
        }

        public async Task<List<ProductResponse>?> AddProductsAsync(List<ProductAddRequest> requests)
        {
            if (requests == null)
                throw new ArgumentNullException(nameof(requests));

            requests.ForEach(ValidationHelper.Validate);
            requests.ForEach(r => ValidationHelper.ValidateEmail(r.ManufactureEmail));

            var products = _mapper.Map<List<Product>>(requests);
            var response = await _productRepository.AddProducstAsync(products);

            return _mapper.Map<List<ProductResponse>>(response);
        }

      
    }
}