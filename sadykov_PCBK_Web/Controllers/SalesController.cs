using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sadykov_PCBK_Web.Data;
using sadykov_PCBK_Web.Models;
using sadykov_PCBK_Web.ViewModels;

namespace sadykov_PCBK_Web.Controllers
{
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SalesController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Add(int? partnerId)
        {
            return View(new SaleAddViewModel
            {
                PartnerId = partnerId,
                SaleDate  = DateTime.Today,
                Partners  = await _db.Partners.OrderBy(p => p.CompanyName).ToListAsync(),
                Products  = await _db.Products.OrderBy(p => p.Article).ToListAsync()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(SaleAddViewModel vm)
        {
            vm.Partners = await _db.Partners.OrderBy(p => p.CompanyName).ToListAsync();
            vm.Products = await _db.Products.OrderBy(p => p.Article).ToListAsync();

            if (!vm.PartnerId.HasValue || vm.PartnerId.Value <= 0)
            { ModelState.AddModelError("", "Выберите партнёра."); return View(vm); }

            if (!vm.ProductId.HasValue || vm.ProductId.Value <= 0)
            { ModelState.AddModelError("", "Выберите продукцию."); return View(vm); }

            if (!vm.Quantity.HasValue || vm.Quantity.Value <= 0)
            { ModelState.AddModelError("", "Укажите количество больше 0."); return View(vm); }

            if (!vm.SaleDate.HasValue)
            { ModelState.AddModelError("", "Укажите дату реализации."); return View(vm); }

            var partnerExists = await _db.Partners.AnyAsync(p => p.Id == vm.PartnerId.Value);
            var productExists = await _db.Products.AnyAsync(p => p.Id == vm.ProductId.Value);

            if (!partnerExists)
            { ModelState.AddModelError("", "Партнёр не найден в базе данных."); return View(vm); }

            if (!productExists)
            { ModelState.AddModelError("", "Продукция не найдена в базе данных."); return View(vm); }

            try
            {
                var saleDate = DateTime.SpecifyKind(vm.SaleDate.Value.Date, DateTimeKind.Utc);

                _db.PartnerSales.Add(new PartnerSale
                {
                    PartnerId = vm.PartnerId.Value,
                    ProductId = vm.ProductId.Value,
                    Quantity  = vm.Quantity.Value,
                    SaleDate  = saleDate
                });
                await _db.SaveChangesAsync();
                TempData["Success"] = "Запись о реализации успешно добавлена.";
                return RedirectToAction("Details", "Partners", new { id = vm.PartnerId.Value });
            }
            catch (Exception ex)
            {
                var inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                ModelState.AddModelError("", $"Ошибка при сохранении: {inner.Message}");
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int partnerId)
        {
            var sale = await _db.PartnerSales.FindAsync(id);
            if (sale != null) { _db.PartnerSales.Remove(sale); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Запись о реализации удалена.";
            return RedirectToAction("Details", "Partners", new { id = partnerId });
        }
    }
}
