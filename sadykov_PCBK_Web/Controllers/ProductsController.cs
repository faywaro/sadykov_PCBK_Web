using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sadykov_PCBK_Web.Data;
using sadykov_PCBK_Web.Models;

namespace sadykov_PCBK_Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ProductsController> _log;

        public ProductsController(ApplicationDbContext db, ILogger<ProductsController> log)
        {
            _db  = db;
            _log = log;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var query = _db.Products.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p =>
                    p.ProductName.ToLower().Contains(search.ToLower()) ||
                    p.Article.ToLower().Contains(search.ToLower()) ||
                    p.ProductType.ToLower().Contains(search.ToLower()));

            ViewBag.Search = search;
            return View(await query.OrderBy(p => p.ProductType).ThenBy(p => p.ProductName).ToListAsync());
        }

        public IActionResult Create() => View(new Product());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid) return View(product);
            try
            {
                _db.Products.Add(product);
                await _db.SaveChangesAsync();
                TempData["Success"] = $"Продукция «{product.ProductName}» добавлена.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ошибка: " + ex.Message);
                return View(product);
            }
        }
        public async Task<IActionResult> Edit(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id) return BadRequest();
            if (!ModelState.IsValid) return View(product);
            try
            {
                _db.Products.Update(product);
                await _db.SaveChangesAsync();
                TempData["Success"] = $"Продукция «{product.ProductName}» обновлена.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ошибка: " + ex.Message);
                return View(product);
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var p = await _db.Products.FindAsync(id);
                if (p != null) { _db.Products.Remove(p); await _db.SaveChangesAsync(); }
                TempData["Success"] = "Продукция удалена.";
            }
            catch (Exception ex) when (ex.Message.Contains("partner_sales") || ex.InnerException?.Message.Contains("partner_sales") == true)
            {
                TempData["Error"] = "Нельзя удалить продукцию — она используется в истории реализации.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ошибка при удалении: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
