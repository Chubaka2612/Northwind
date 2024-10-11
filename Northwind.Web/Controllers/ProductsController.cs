using Microsoft.AspNetCore.Mvc;
using Northwind.Bll.Abstractions;
using Northwind.Domain.Entities;
using Northwind.Web.Models;

namespace Northwind.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ISupplierService _supplierService;
        private static int _amount;

        public ProductsController(IConfiguration configuration, IProductService productService, ICategoryService categoryService, ISupplierService supplierService)
        {
            _amount = configuration != null
                ? configuration.GetValue<int>("ProductSettings:MaxProductCount")
                : throw new ArgumentNullException(nameof(configuration));
            _productService = productService;
            _categoryService = categoryService;
            _supplierService = supplierService; 
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            return View(await _productService.GetProductsAsync(cancellationToken, _amount));
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var product = new ProductModel
            {
                Product = new Product(),
                Categories = await _categoryService.GetCategoriesAsync(cancellationToken),
                Suppliers = await _supplierService.GetSuppliersAsync(cancellationToken)
            };
            return View(product);
        }

        [HttpPost]
        public IActionResult Create(ProductModel productModel, CancellationToken cancellationToken)
        {
            _productService.AddProductAsync(productModel.Product);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
        {
            if (id is null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductAsync((int)id, cancellationToken);
            if (product is null)
            {
                return NotFound();
            }

            var categories = await _categoryService.GetCategoriesAsync(cancellationToken);
            var suppliers = await _supplierService.GetSuppliersAsync(cancellationToken);

            if (categories is null || suppliers is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error while fetching related data.");
            }

            var productModel = new ProductModel
            {
                Product = product,
                Categories = categories,
                Suppliers = suppliers
            };

            return View(productModel);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductModel productModel)
        {
            var product = productModel.Product;
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _productService.UpdateProductAsync(product);
                return RedirectToAction(nameof(Index));
            }

            return View(productModel);
        }

    }
}
