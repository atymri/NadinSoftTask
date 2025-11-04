using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ProductManager.Core.Domain.Entities;
using ProductManager.Core.Domain.RepositoryContracts;
using ProductManager.Core.DTOs.ProductDTOs;
using ProductManager.Core.ServiceContracts;

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
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null) 
                throw new ArgumentNullException(nameof(product));
            var res = await _productRepository.DeleteProductAsync(product.ID);
            return res;
        }
        public async Task<bool> DeleteProductsAsync(List<ProductResponse?> products)
        {
            if (products == null)
                throw new ArgumentNullException(nameof(products));

            var entities = _mapper.Map<List<Product>>(products);

            var res = await _productRepository.DeleteProductsAsync(entities);
            return res;
        }
        public async Task<bool> DeleteProductsBeforeThan(DateTime date)
        {
            var products = await _productRepository.GetAllProductsAsync();
            if (products == null || products.Count == 0)
                throw new ArgumentNullException(nameof(products));

            var productsToDelete = products.Where(p => p.Date < date).ToList();
            var res = await _productRepository.DeleteProductsAsync(productsToDelete);
            return res;
        }
    }
}
