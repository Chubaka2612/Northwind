
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
    }
}
