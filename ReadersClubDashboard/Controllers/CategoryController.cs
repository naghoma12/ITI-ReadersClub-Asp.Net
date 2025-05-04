using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadersClubCore.Models;
using ReadersClubDashboard.Service;

namespace ReadersClubDashboard.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: Index
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }

        // GET: Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {

                var existingCategory = await _categoryService.GetByNameAsync(category.Name);

                if (existingCategory != null && existingCategory.IsDeleted)
                {

                    existingCategory.IsDeleted = false;
                    await _categoryService.UpdateAsync(existingCategory);
                    return RedirectToAction(nameof(Index));
                }

                if (existingCategory != null && !existingCategory.IsDeleted)
                {

                    ModelState.AddModelError(string.Empty, "التصنيف موجود بالفعل!");
                    return View(category);
                }


                await _categoryService.AddAsync(category);
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = await _categoryService.GetByIdAsync(id.Value);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _categoryService.UpdateAsync(category);
                }
                catch
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var category = await _categoryService.GetByIdAsync(id.Value);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Confirm Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryService.SoftDeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
