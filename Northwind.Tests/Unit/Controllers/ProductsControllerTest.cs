
using Microsoft.AspNetCore.Mvc;
using Moq;
using Northwind.Bll.Abstractions;
using Northwind.Web.Controllers;
using NUnit.Framework;
using Northwind.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace Northwind.Tests.Unit.Controllers;

[TestFixture]
public class ProductsControllerTests
{
    private Mock<IProductService> _mockProductService;
    private Mock<ICategoryService> _mockCategoryService;
    private Mock<ISupplierService> _mockSupplierService;
    private Mock<ILogger<ProductsController>> _mockLogger;
    private IConfiguration _mockConfiguration;
    private ProductsController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockProductService = new Mock<IProductService>();
        _mockCategoryService = new Mock<ICategoryService>();
        _mockSupplierService = new Mock<ISupplierService>();
        _mockLogger = new Mock<ILogger<ProductsController>>();
        _mockConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                    { "ProductSettings:MaxProductCount", "10" }
            })
            .Build();

        _controller = new ProductsController(
            _mockConfiguration,
            _mockProductService.Object,
            _mockCategoryService.Object,
            _mockSupplierService.Object,
            _mockLogger.Object
        );
    }

    #region Positive Tests

    [Test]
    public async Task GetProducts_ReturnsOkWithProducts()
    {
        // Arrange
        var sampleProducts = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Product A" },
                new Product { ProductId = 2, ProductName = "Product B" }
            };
        _mockProductService.Setup(s => s.GetProductsAsync(It.IsAny<CancellationToken>(), It.IsAny<int>()))
                           .ReturnsAsync(sampleProducts);

        // Act
        var result = await _controller.GetProducts(CancellationToken.None) as OkObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(result.Value, Is.Not.Null);
    }

    [Test]
    public async Task GetProduct_ValidId_ReturnsOk()
    {
        // Arrange
        var sampleProduct = new Product { ProductId = 1, ProductName = "Product A" };
        _mockProductService.Setup(s => s.GetProductAsync(1, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(sampleProduct);

        // Act
        var result = await _controller.GetProduct(1, CancellationToken.None) as OkObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(result.Value, Is.EqualTo(sampleProduct));
    }

    [Test]
    public async Task PostProduct_ValidProduct_ReturnsCreated()
    {
        // Arrange
        var newProduct = new Product { ProductName = "New Product", SupplierId = 1, CategoryId = 1 };
        _mockProductService.Setup(s => s.AddProductAsync(newProduct))
                           .ReturnsAsync(1);

        // Act
        var result = await _controller.PostProduct(newProduct) as CreatedAtActionResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(201));
        Assert.That(result.Value, Is.EqualTo(1));
    }

    [Test]
    public async Task PutProduct_ValidProduct_ReturnsNoContent()
    {
        // Arrange
        var updatedProduct = new Product { ProductId = 1, ProductName = "Updated Product" };
        _mockProductService.Setup(s => s.UpdateProductAsync(updatedProduct))
                           .ReturnsAsync(true);

        // Act
        var result = await _controller.PutProduct(1, updatedProduct) as NoContentResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(204));
    }

    [Test]
    public async Task DeleteProduct_ValidId_ReturnsNoContent()
    {
        // Arrange
        var existingProduct = new Product { ProductId = 1 };
        _mockProductService.Setup(s => s.GetProductAsync(1, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(existingProduct);
        _mockProductService.Setup(s => s.DeleteProductAsync(existingProduct))
                           .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteProduct(1) as NoContentResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(204));
    }

    #endregion

    #region Negative Tests

    [Test]
    public async Task GetProduct_InvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockProductService.Setup(s => s.GetProductAsync(99, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((Product)null);

        // Act
        var result = await _controller.GetProduct(99, CancellationToken.None) as NotFoundObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(404));
        Assert.That(result.Value, Is.EqualTo("Product with ID 99 not found."));
    }

    [Test]
    public async Task PostProduct_NullProduct_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.PostProduct(null) as BadRequestObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public async Task PutProduct_IdMismatch_ReturnsBadRequest()
    {
        // Arrange
        var product = new Product { ProductId = 1, ProductName = "Product A" };

        // Act
        var result = await _controller.PutProduct(2, product) as BadRequestObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(400));
        Assert.That(result.Value, Is.EqualTo("Product ID mismatch."));
    }

    [Test]
    public async Task DeleteProduct_InvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockProductService.Setup(s => s.GetProductAsync(99, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((Product)null);

        // Act
        var result = await _controller.DeleteProduct(99) as NotFoundObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(404));
        Assert.That(result.Value, Is.EqualTo("Product with ID 99 not found."));
    }

    [Test]
    public async Task GetProducts_ThrowsException_ReturnsServerError()
    {
        // Arrange
        _mockProductService.Setup(s => s.GetProductsAsync(It.IsAny<CancellationToken>(), It.IsAny<int>()))
                           .ThrowsAsync(new System.Exception("Database error"));

        // Act
        var result = await _controller.GetProducts(CancellationToken.None) as ObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(500));
        Assert.That(result.Value, Is.EqualTo("An error occurred while fetching products."));
    }

    #endregion
}