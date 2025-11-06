using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManager.API.Controllers;
using ProductManager.Core.DTOs.ProductDTOs;
using ProductManager.Core.ServiceContracts;
using System.Security.Claims;

namespace ProductManagement.Tests.ControllerTests
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductGetterService> _mockGetterService;
        private readonly Mock<IProductAdderService> _mockAdderService;
        private readonly Mock<IProductUpdaterService> _mockUpdaterService;
        private readonly Mock<IProductDeleterService> _mockDeleterService;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mockGetterService = new Mock<IProductGetterService>();
            _mockAdderService = new Mock<IProductAdderService>();
            _mockUpdaterService = new Mock<IProductUpdaterService>();
            _mockDeleterService = new Mock<IProductDeleterService>();

            _controller = new ProductController(
                _mockGetterService.Object,
                _mockAdderService.Object,
                _mockUpdaterService.Object,
                _mockDeleterService.Object
            );
        }

        private void SetUserEmail(string email)
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, email)
                    }, "TestAuth"))
                }
            };
        }

        #region GetAllProducts
        [Fact]
        public async Task GetAllProducts_ReturnsOkWithProducts()
        {
            var products = new List<ProductResponse>
            {
                new ProductResponse { ID = Guid.NewGuid(), Name = "Product1" },
                new ProductResponse { ID = Guid.NewGuid(), Name = "Product2" }
            };
            _mockGetterService.Setup(s => s.GetAllProductsAsync()).ReturnsAsync(products);

            var result = await _controller.GetAllProducts();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsType<List<ProductResponse>>(okResult.Value);
            Assert.Equal(2, returnedProducts.Count);
            Assert.Equal(products[0].ID, returnedProducts[0].ID);
        }
        #endregion

        #region GetProductById
        [Fact]
        public async Task GetProductById_ReturnsOkWithProduct()
        {
            var id = Guid.NewGuid();
            var product = new ProductResponse { ID = id, Name = "TestProduct" };
            _mockGetterService.Setup(s => s.GetProductByIdAsync(id)).ReturnsAsync(product);

            var result = await _controller.GetProductById(id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<ProductResponse>(okResult.Value);
            Assert.Equal(id, returnedProduct.ID);
        }
        #endregion

        #region PostProduct
        [Fact]
        public async Task PostProduct_ValidEmail_ReturnsOk()
        {
            var request = new ProductAddRequest { ManufactureEmail = "user@gmail.com", Name = "NewProduct" };
            SetUserEmail("user@gmail.com");

            var response = new ProductResponse { ID = Guid.NewGuid(), Name = "NewProduct" };
            _mockAdderService.Setup(s => s.AddProductAsync(request)).ReturnsAsync(response);

            var result = await _controller.PostProduct(request);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<ProductResponse>(okResult.Value);
            Assert.Equal(response.ID, returnedProduct.ID);
        }

        [Fact]
        public async Task PostProduct_InvalidEmailDomain_ReturnsForbidden()
        {
            var request = new ProductAddRequest { ManufactureEmail = "user@notallowed.com", Name = "NewProduct" };
            SetUserEmail("user@gmail.com");

            var result = await _controller.PostProduct(request);

            var problemResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status403Forbidden, problemResult.StatusCode);
        }

        [Fact]
        public async Task PostProduct_InvalidModel_ReturnsValidationProblem()
        {
            var request = new ProductAddRequest { ManufactureEmail = "", Name = "" };
            _controller.ModelState.AddModelError("Email", "Required");

            var result = await _controller.PostProduct(request);

            Assert.IsType<ValidationProblemDetails>(Assert.IsType<ObjectResult>(result.Result).Value);
        }
        #endregion

        #region PutProduct
        [Fact]
        public async Task PutProduct_ValidEmail_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var request = new ProductUpdateRequest { ManufactureEmail = "user@gmail.com", Name = "UpdatedProduct" };
            SetUserEmail("user@gmail.com");

            var response = new ProductResponse { ID = id, Name = "UpdatedProduct" };
            _mockUpdaterService.Setup(s => s.UpdateProductAsync(id, request)).ReturnsAsync(response);

            var result = await _controller.PutProduct(request, id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<ProductResponse>(okResult.Value);
            Assert.Equal(response.ID, returnedProduct.ID);
        }

        [Fact]
        public async Task PutProduct_InvalidEmail_ReturnsForbidden()
        {
            var id = Guid.NewGuid();
            var request = new ProductUpdateRequest { ManufactureEmail = "other@gmail.com", Name = "UpdatedProduct" };
            SetUserEmail("user@gmail.com");

            var result = await _controller.PutProduct(request, id);

            var problemResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status403Forbidden, problemResult.StatusCode);
        }

        [Fact]
        public async Task PutProduct_InvalidModel_ReturnsValidationProblem()
        {
            var id = Guid.NewGuid();
            var request = new ProductUpdateRequest { ManufactureEmail = "", Name = "" };
            _controller.ModelState.AddModelError("Email", "Required");

            var result = await _controller.PutProduct(request, id);

            Assert.IsType<ValidationProblemDetails>(Assert.IsType<ObjectResult>(result.Result).Value);
        }
        #endregion

        #region DeleteProduct
        [Fact]
        public async Task DeleteProduct_ProductNotFound_ReturnsNotFound()
        {
            var id = Guid.NewGuid();
            _mockGetterService.Setup(s => s.GetProductByIdAsync(id)).ReturnsAsync((ProductResponse)null);

            var result = await _controller.DeleteProduct(id);

            var problemResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status404NotFound, problemResult.StatusCode);
        }

        [Fact]
        public async Task DeleteProduct_NotOwner_ReturnsForbidden()
        {
            var id = Guid.NewGuid();
            var product = new ProductResponse { ID = id, ManufactureEmail = "other@gmail.com" };
            _mockGetterService.Setup(s => s.GetProductByIdAsync(id)).ReturnsAsync(product);
            SetUserEmail("user@gmail.com");

            var result = await _controller.DeleteProduct(id);

            var problemResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status403Forbidden, problemResult.StatusCode);
        }

        [Fact]
        public async Task DeleteProduct_Valid_ReturnsOkTrue()
        {
            var id = Guid.NewGuid();
            var product = new ProductResponse { ID = id, ManufactureEmail = "user@gmail.com" };
            _mockGetterService.Setup(s => s.GetProductByIdAsync(id)).ReturnsAsync(product);
            _mockDeleterService.Setup(s => s.DeleteProductAsync(id)).ReturnsAsync(true);
            SetUserEmail("user@gmail.com");

            var result = await _controller.DeleteProduct(id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.True((bool)okResult.Value);
        }
        #endregion
    }
}
