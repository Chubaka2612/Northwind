
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Northwind.Bll.Abstractions;
using Northwind.Web.Controllers;
using NUnit.Framework;
using Northwind.Domain.Entities;
using Northwind.Web.Models;

namespace Northwind.Tests.Unit.Controllers;

[TestFixture]
public class ProductsControllerTests
{
    private ProductsController _controller;
    private Mock<IProductService> _mockProductService;
    private Mock<ICategoryService> _mockCategoryService;
    private Mock<ISupplierService> _mockSupplierService;
    private IConfiguration _configuration;

    [SetUp]
    public void Setup()
    {
       
        var inMemorySettings = new Dictionary<string, string> { { "ProductSettings:MaxProductCount", "10" } };
        _configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

        _mockProductService = new Mock<IProductService>();
        _mockCategoryService = new Mock<ICategoryService>();
        _mockSupplierService = new Mock<ISupplierService>();

      
        _controller = new ProductsController(_configuration, _mockProductService.Object, _mockCategoryService.Object, _mockSupplierService.Object);
    }

    [Test]
    public async Task Index_ReturnsViewResult_WithProductList()
    {
        // Arrange
        var products = new List<Product> { new Product() }; 
        _mockProductService.Setup(service => service.GetProductsAsync(It.IsAny<CancellationToken>(), It.IsAny<int>()))
                           .ReturnsAsync(products);

        // Act
        var result = await _controller.Index(CancellationToken.None) as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(products, Is.EqualTo(result.Model));
    }

    [Test]
    public async Task Create_GET_ReturnsViewResult_WithProductModel()
    {
        // Arrange
        _mockCategoryService.Setup(service => service.GetCategoriesAsync(It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new List<Category>());
        _mockSupplierService.Setup(service => service.GetSuppliersAsync(It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new List<Supplier>());

        // Act
        var result = await _controller.Create(CancellationToken.None) as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        var model = result.Model as ProductModel;
        Assert.That(model, Is.Not.Null);
        Assert.That(model.Categories, Is.Not.Null);
        Assert.That(model.Suppliers, Is.Not.Null);
    }

    [Test]
    public async Task Create_POST_RedirectsToIndex_WhenModelIsValid()
    {
        // Arrange
        var productModel = new ProductModel { Product = new Product() };
        _mockProductService.Setup(service => service.AddProductAsync(It.IsAny<Product>()));

        // Act
        var result = _controller.Create(productModel, CancellationToken.None) as RedirectToActionResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That("Index", Is.EqualTo(result.ActionName));
        _mockProductService.Verify(service => service.AddProductAsync(productModel.Product), Times.Once);
    }

    [Test]
    public async Task Edit_GET_ReturnsNotFound_WhenIdIsNull()
    {
        // Act
        var result = await _controller.Edit(null, CancellationToken.None) as NotFoundResult;

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task Edit_GET_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        _mockProductService.Setup(service => service.GetProductAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((Product)null);

        // Act
        var result = await _controller.Edit(1, CancellationToken.None) as NotFoundResult;

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task Edit_GET_ReturnsViewResult_WithProductModel()
    {
        // Arrange
        var product = new Product { ProductId = 1 };
        _mockProductService.Setup(service => service.GetProductAsync(1, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(product);
        _mockCategoryService.Setup(service => service.GetCategoriesAsync(It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new List<Category>());
        _mockSupplierService.Setup(service => service.GetSuppliersAsync(It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new List<Supplier>());

        // Act
        var result = await _controller.Edit(1, CancellationToken.None) as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        var model = result.Model as ProductModel;
        Assert.That(model, Is.Not.Null);
        Assert.That(product, Is.EqualTo(model.Product));
    }

    [Test]
    public async Task Edit_POST_RedirectsToIndex_WhenModelIsValid()
    {
        // Arrange
        var productModel = new ProductModel { Product = new Product { ProductId = 1 } };
        _mockProductService.Setup(service => service.UpdateProductAsync(It.IsAny<Product>()));

        // Act
        var result = await _controller.Edit(1, productModel) as RedirectToActionResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That("Index", Is.EqualTo(result.ActionName));
        _mockProductService.Verify(service => service.UpdateProductAsync(productModel.Product), Times.Once);
    }

    [Test]
    public async Task Edit_POST_ReturnsViewResult_WhenModelIsInvalid()
    {
        // Arrange
        var productModel = new ProductModel { Product = new Product { ProductId = 1 } };
        _controller.ModelState.AddModelError("Error", "Model is invalid");

        // Act
        var result = await _controller.Edit(1, productModel) as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(productModel, Is.EqualTo(result.Model));
    }
}
