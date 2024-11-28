using Microsoft.AspNetCore.Mvc;
using Northwind.Bll.Abstractions;
using Northwind.Domain.Entities;

namespace Northwind.Web.Controllers
{
    [ApiController]
    [Route("api/northwind/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ISupplierService _supplierService;
        private readonly ILogger<ProductsController> _logger;
        private static int _amount;

        public ProductsController(
            IConfiguration configuration,
            IProductService productService,
            ICategoryService categoryService,
            ISupplierService supplierService,
            ILogger<ProductsController> logger)
        {
            _amount = configuration?.GetValue<int>("ProductSettings:MaxProductCount")
                     ?? throw new ArgumentNullException(nameof(configuration));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(CancellationToken cancellationToken)
        {
            try
            {
            
                var products = await _productService.GetProductsAsync(cancellationToken, _amount);
              
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products");
                return StatusCode(500, "An error occurred while fetching products.");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProduct([FromRoute] int id, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _productService.GetProductAsync(id, cancellationToken);
                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found.");
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product by ID");
                return StatusCode(500, "An error occurred while fetching the product.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] Product product)
        {
            if (product is null)
            {
                return BadRequest("Product can't be null.");
            }

            try
            {
                var productId = await _productService.AddProductAsync(product);
                return CreatedAtAction(nameof(GetProduct), new { id = productId }, productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product");
                return StatusCode(500, "An error occurred while adding the product.");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutProduct([FromRoute] int id, [FromBody] Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest("Product ID mismatch.");
            }

            try
            {
                var isUpdated = await _productService.UpdateProductAsync(product);
                if (!isUpdated)
                {
                    return NotFound($"Product with ID {id} not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                return StatusCode(500, "An error occurred while updating the product.");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            try
            {
                var product = await _productService.GetProductAsync(id, CancellationToken.None);
                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found.");
                }

                var isDeleted = await _productService.DeleteProductAsync(product);
                if (!isDeleted)
                {
                    return StatusCode(500, "An error occurred while deleting the product.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product");
                return StatusCode(500, "An error occurred while deleting the product.");
            }
        }
    }
}
