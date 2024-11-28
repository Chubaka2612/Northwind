
using Microsoft.AspNetCore.Mvc;
using Northwind.Bll.Abstractions;

namespace Northwind.Web.Controllers
{

    [ApiController]
    [Route("api/northwind/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ILogger<CategoriesController> _logger;
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
        {
            try
            {
                var categories = await _categoryService.GetCategoriesAsync(cancellationToken);
                if (categories == null || !categories.Any())
                {
                    _logger.LogWarning("No categories found.");
                    return NotFound("No categories available.");
                }

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching categories");
                return StatusCode(500, "An error occurred while fetching categories.");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategory([FromRoute] int id, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _categoryService.GetCategoryAsync(id, cancellationToken);
                if (category == null)
                {
                    _logger.LogWarning($"Category with ID {id} not found.");
                    return NotFound($"Category with ID {id} not found.");
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching category by ID {id}");
                return StatusCode(500, "An error occurred while fetching the category.");
            }
        }

        [HttpPut("{id:int}/image")]
        public async Task<IActionResult> UpdateCategoryImage([FromRoute] int id, [FromForm] IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning($"Invalid file input for category ID {id}.");
                return BadRequest("File cannot be null or empty.");
            }

            try
            {
                var category = await _categoryService.GetCategoryAsync(id, cancellationToken);
                if (category == null)
                {
                    _logger.LogWarning($"Category with ID {id} not found.");
                    return NotFound($"Category with ID {id} not found.");
                }

                byte[] imageData;
                using (var binaryReader = new BinaryReader(file.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)file.Length);
                }

                category.Picture = imageData;
                await _categoryService.UpdateCategoryImageAsync(id, imageData, cancellationToken);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating image for category ID {id}");
                return StatusCode(500, "An error occurred while updating the category image.");
            }
        }
    }
}