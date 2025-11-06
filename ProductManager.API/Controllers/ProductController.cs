using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Core.DTOs.ProductDTOs;
using ProductManager.Core.ServiceContracts;

namespace ProductManager.API.Controllers
{
    [AllowAnonymous]
    public class ProductController : BaseController
    {
        private readonly IProductGetterService _productGetterService;
        private readonly IProductAdderService _productAdderService;
        private readonly IProductUpdaterService _productUpdaterService;
        private readonly IProductDeleterService _productDeleterService;

        public ProductController(IProductGetterService getterService, IProductAdderService adderService, 
            IProductUpdaterService productUpdaterService, IProductDeleterService productDeleterService)
        {
            _productGetterService = getterService;
            _productAdderService = adderService;
            _productUpdaterService = productUpdaterService;
            _productDeleterService = productDeleterService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductResponse>>> GetAllProducts()
        {
            var products = await _productGetterService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponse>> GetProductById(Guid id)
        {
            var product = await _productGetterService.GetProductByIdAsync(id);
            return Ok(product);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ProductResponse>> PostProduct(ProductAddRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (request.ManufactureEmail != User.Identity.Name)
                return Problem("شما فقط میتوانید برای خودتان محصول اضافه کنید",
                    statusCode: StatusCodes.Status403Forbidden);

            var res = await _productAdderService.AddProductAsync(request);
            return Ok(res);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductResponse>> PutProduct(ProductUpdateRequest request, Guid id)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (request.ManufactureEmail != User.Identity.Name)
                return Problem("شما فقط میتوانید محصولات خودتان را ویرایش کنید.",
                    statusCode: StatusCodes.Status403Forbidden);

            var res = await _productUpdaterService.UpdateProductAsync(id, request);
            return Ok(res);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteProduct(Guid id)
        {
            var product = await _productGetterService.GetProductByIdAsync(id);
            if (product == null)
                return Problem("هیچ محصولی با آیدی داده شده یافت نشد", statusCode: StatusCodes.Status404NotFound);

            if (product.ManufactureEmail != User.Identity.Name)
                return Problem("شما فقط میتوانید محصولات خودتان را حذف کنید.",
            
                    statusCode: StatusCodes.Status403Forbidden);

            var res = await _productDeleterService.DeleteProductAsync(id);
            if(!res)
                return Ok(false);
            return Ok(true);
        }




    }
}
