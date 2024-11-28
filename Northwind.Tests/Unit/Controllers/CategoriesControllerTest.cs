using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Northwind.Bll.Abstractions;
using Northwind.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Northwind.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Northwind.Tests.Unit.Controllers;

[TestFixture]
public class CategoriesControllerTests
{
    private Mock<ICategoryService> _categoryServiceMock;
    private Mock<ILogger<CategoriesController>> _loggerMock;
    private CategoriesController _controller;

    [SetUp]
    public void Setup()
    {
        _categoryServiceMock = new Mock<ICategoryService>();
        _loggerMock = new Mock<ILogger<CategoriesController>>();
        _controller = new CategoriesController(_categoryServiceMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task GetCategories_ShouldReturnOk_WhenCategoriesExist()
    {
        // Arrange
        var categories = new List<Category>
            {
                new Category { CategoryId = 1, CategoryName = "Beverages" },
                new Category { CategoryId = 2, CategoryName = "Condiments" }
            };
        _categoryServiceMock
            .Setup(service => service.GetCategoriesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        // Act
        var result = await _controller.GetCategories(CancellationToken.None);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(categories));
    }

    [Test]
    public async Task GetCategories_ShouldReturnNotFound_WhenNoCategoriesExist()
    {
        // Arrange
        _categoryServiceMock
            .Setup(service => service.GetCategoriesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category>());

        // Act
        var result = await _controller.GetCategories(CancellationToken.None);

        // Assert
        Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult.Value, Is.EqualTo("No categories available."));
    }

    [Test]
    public async Task GetCategories_ShouldReturnInternalServerError_OnException()
    {
        // Arrange
        _categoryServiceMock
            .Setup(service => service.GetCategoriesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new System.Exception("Database error"));

        // Act
        var result = await _controller.GetCategories(CancellationToken.None);

        // Assert
        Assert.That(result, Is.TypeOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        Assert.That(objectResult.Value, Is.EqualTo("An error occurred while fetching categories."));
    }

    [Test]
    public async Task GetCategory_ShouldReturnOk_WhenCategoryExists()
    {
        // Arrange
        var category = new Category { CategoryId = 1, CategoryName = "Beverages" };
        _categoryServiceMock
            .Setup(service => service.GetCategoryAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _controller.GetCategory(1, CancellationToken.None);

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(category));
    }

    [Test]
    public async Task GetCategory_ShouldReturnNotFound_WhenCategoryDoesNotExist()
    {
        // Arrange
        _categoryServiceMock
            .Setup(service => service.GetCategoryAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category)null);

        // Act
        var result = await _controller.GetCategory(1, CancellationToken.None);

        // Assert
        Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult.Value, Is.EqualTo("Category with ID 1 not found."));
    }

    [Test]
    public async Task UpdateCategoryImage_ShouldReturnNoContent_WhenImageIsUpdatedSuccessfully()
    {
        // Arrange
        var category = new Category { CategoryId = 1 };
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(file => file.Length).Returns(1024);
        mockFile.Setup(file => file.OpenReadStream()).Returns(new System.IO.MemoryStream(new byte[1024]));

        _categoryServiceMock
            .Setup(service => service.GetCategoryAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _categoryServiceMock
            .Setup(service => service.UpdateCategoryImageAsync(1, It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true); 

        // Act
        var result = await _controller.UpdateCategoryImage(1, mockFile.Object, CancellationToken.None);

        // Assert
        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task UpdateCategoryImage_ShouldReturnNotFound_WhenCategoryDoesNotExist()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(file => file.Length).Returns(1024);
        mockFile.Setup(file => file.OpenReadStream()).Returns(new System.IO.MemoryStream(new byte[1024]));
        _categoryServiceMock
            .Setup(service => service.GetCategoryAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category)null);

        // Act
        var result = await _controller.UpdateCategoryImage(1, mockFile.Object, CancellationToken.None);

        // Assert
        Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult.Value, Is.EqualTo("Category with ID 1 not found."));
    }

    [Test]
    public async Task UpdateCategoryImage_ShouldReturnBadRequest_WhenFileIsNullOrEmpty()
    {
        // Arrange
        IFormFile file = null;

        // Act
        var result = await _controller.UpdateCategoryImage(1, file, CancellationToken.None);

        // Assert
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult.Value, Is.EqualTo("File cannot be null or empty."));
    }
}
