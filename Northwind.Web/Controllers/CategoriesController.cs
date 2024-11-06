
using Microsoft.AspNetCore.Mvc;
using Northwind.Bll.Abstractions;

namespace Northwind.Web.Controllers
{
  
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
   
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            return View(await _categoryService.GetCategoriesAsync(cancellationToken));
        }

        [Route("Image/{id?}")]
        [Route("{controller}/{action}/{id?}")]
        public async Task<IActionResult> Image([FromRoute] int? id)
        {
            var category = (await _categoryService.GetCategoriesAsync(CancellationToken.None))
                           .FirstOrDefault(c => c.CategoryId == id);

            if (category == null || category.Picture == null)
            {
                return NotFound();
            }

            // Skip first 78 bytes if the image is corrupted
            var fixedImage = category.Picture.Skip(78).ToArray();

            return File(fixedImage, "image/bmp");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryAsync(id, CancellationToken.None);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, IFormFile newImage)
        {
            if (newImage != null && newImage.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await newImage.CopyToAsync(stream);
                    var imageBytes = stream.ToArray();
                    await _categoryService.UpdateCategoryImageAsync(id, imageBytes, CancellationToken.None);
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

