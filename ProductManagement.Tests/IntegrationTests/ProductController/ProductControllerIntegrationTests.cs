using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManager.API.Controllers;
using ProductManager.Core.DTOs.ProductDTOs;
using ProductManager.Core.ServiceContracts;

public class ProductControllerAnonymousTests
{
    private readonly Mock<IProductGetterService> _getterMock = new();
    private readonly Mock<IProductAdderService> _adderMock = new();
    private readonly Mock<IProductUpdaterService> _updaterMock = new();
    private readonly Mock<IProductDeleterService> _deleterMock = new();
    private readonly ProductController _controller;

    public ProductControllerAnonymousTests()
    {
        _controller = new ProductController(
            _getterMock.Object,
            _adderMock.Object,
            _updaterMock.Object,
            _deleterMock.Object
        );

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() // User == null
        };
    }

    [Fact]
    public async Task PostProduct_Anonymous_ReturnsForbidden()
    {
        var request = new ProductAddRequest { ManufactureEmail = "user@gmail.com", Name = "Test" };
        var result = await _controller.PostProduct(request);
        var objResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status403Forbidden, objResult.StatusCode);
    }

    [Fact]
    public async Task PutProduct_Anonymous_ReturnsForbidden()
    {
        var request = new ProductUpdateRequest { ManufactureEmail = "user@gmail.com", Name = "Test" };
        var result = await _controller.PutProduct(request, Guid.NewGuid());
        var objResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status403Forbidden, objResult.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_Anonymous_ReturnsForbidden()
    {
        var productId = Guid.NewGuid();

        var product = new ProductResponse
        {
            ID = productId,
            Name = "TestProduct",
            ManufactureEmail = "user@gmail.com"
        };

        var getterMock = new Mock<IProductGetterService>();
        getterMock.Setup(s => s.GetProductByIdAsync(productId)).ReturnsAsync(product);

        var controller = new ProductController(
            getterMock.Object,
            new Mock<IProductAdderService>().Object,
            new Mock<IProductUpdaterService>().Object,
            new Mock<IProductDeleterService>().Object
        );

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() // User == null
        };

        var result = await controller.DeleteProduct(productId);
        var objResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status403Forbidden, objResult.StatusCode);
    }

}
