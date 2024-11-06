using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Northwind.Bll.Abstractions;
using Northwind.Web.Controllers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Northwind.Domain.Entities;
using NUnit.Framework.Legacy;

namespace Northwind.Tests.Unit.Controllers;

[TestFixture]
public class CategoriesControllerTests
{
    private CategoriesController _controller;
    private Mock<ICategoryService> _mockCategoryService;

    [SetUp]
    public void Setup()
    {
        _mockCategoryService = new Mock<ICategoryService>();

        _controller = new CategoriesController(_mockCategoryService.Object);
    }

    [Test]
    public async Task Index_ReturnsViewResult_WithCategoryList()
    {
        // Arrange
        var categories = new List<Category> { new Category { CategoryId = 1 } };
        _mockCategoryService.Setup(service => service.GetCategoriesAsync(It.IsAny<CancellationToken>()))
                            .ReturnsAsync(categories);

        // Act
        var result = await _controller.Index(CancellationToken.None) as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(categories, Is.EqualTo(result.Model));
    }

    [Test]
    public async Task Image_ReturnsNotFound_WhenCategoryIsNull()
    {
        // Arrange
        _mockCategoryService.Setup(service => service.GetCategoriesAsync(It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new List<Category>());

        // Act
        var result = await _controller.Image(1) as NotFoundResult;

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task Image_ReturnsNotFound_WhenCategoryPictureIsNull()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { CategoryId = 1, Picture = null }
        };
        _mockCategoryService.Setup(service => service.GetCategoriesAsync(It.IsAny<CancellationToken>()))
                            .ReturnsAsync(categories);

        // Act
        var result = await _controller.Image(1) as NotFoundResult;

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task Image_ReturnsFileResult_WhenImageExists()
    {
        // Arrange
        var category = new Category
        {
            CategoryId = 1,
            Picture = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]
        };
        _mockCategoryService.Setup(service => service.GetCategoriesAsync(It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new List<Category> { category });

        // Act
        var result = await _controller.Image(1) as FileResult;

        // Assert   
        Assert.That(result, Is.Not.Null);
        Assert.That("image/bmp", Is.EqualTo(result.ContentType));
    }

    [Test]
    public async Task Edit_GET_ReturnsNotFound_WhenCategoryDoesNotExist()
    {
        // Arrange
        _mockCategoryService.Setup(service => service.GetCategoryAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                            .ReturnsAsync((Category)null);

        // Act
        var result = await _controller.Edit(1) as NotFoundResult;

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task Edit_GET_ReturnsViewResult_WithCategory()
    {
        // Arrange
        var category = new Category { CategoryId = 1 };
        _mockCategoryService.Setup(service => service.GetCategoryAsync(1, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(category);

        // Act
        var result = await _controller.Edit(1) as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(category, Is.EqualTo(result.Model));
    }

    [Test]
    public async Task Edit_POST_RedirectsToIndex_WhenImageIsUploaded()
    {
        // Arrange
        var categoryId = 1;
        var imageFile = new Mock<IFormFile>();
        var memoryStream = new MemoryStream(new byte[] { 0, 1, 2, 3 });
        imageFile.Setup(f => f.Length).Returns(memoryStream.Length);
        imageFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                 .Callback<Stream, CancellationToken>((stream, token) => memoryStream.CopyTo(stream));

        // Act
        var result = await _controller.Edit(categoryId, imageFile.Object) as RedirectToActionResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That("Index", Is.EqualTo(result.ActionName));
        _mockCategoryService.Verify(service => service.UpdateCategoryImageAsync(categoryId, It.IsAny<byte[]>(), CancellationToken.None), Times.Once);
    }

    [Test]
    public async Task Edit_POST_RedirectsToIndex_WhenNoImageIsUploaded()
    {
        // Arrange
        var categoryId = 1;
        var imageFile = (IFormFile)null; // No file

        // Act
        var result = await _controller.Edit(categoryId, imageFile) as RedirectToActionResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That("Index", Is.EqualTo(result.ActionName));
        _mockCategoryService.Verify(service => service.UpdateCategoryImageAsync(It.IsAny<int>(), It.IsAny<byte[]>(), CancellationToken.None), Times.Never);
    }
}
