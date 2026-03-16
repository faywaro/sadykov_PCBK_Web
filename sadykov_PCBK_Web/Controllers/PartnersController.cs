using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sadykov_PCBK_Web.Data;
using sadykov_PCBK_Web.Models;
using sadykov_PCBK_Web.ViewModels;

namespace sadykov_PCBK_Web.Controllers
{
    public class PartnersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<PartnersController> _log;

        public PartnersController(ApplicationDbContext db, ILogger<PartnersController> log)
        {
            _db  = db;
            _log = log;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var query = _db.Partners
                .Include(p => p.PartnerType)
                .Include(p => p.Sales)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p =>
                    p.CompanyName.ToLower().Contains(search.ToLower()) ||
                    p.Inn.Contains(search) ||
                    p.DirectorName.ToLower().Contains(search.ToLower()));

            var list = await query.OrderBy(p => p.CompanyName).ToListAsync();

            return View(new PartnersIndexViewModel
            {
                Partners   = list.Select(p => new PartnerViewModel(p)).ToList(),
                SearchTerm = search
            });
        }

        public async Task<IActionResult> Details(int id)
        {
            var partner = await _db.Partners
                .Include(p => p.PartnerType)
                .Include(p => p.Sales).ThenInclude(s => s.Product)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (partner == null) return NotFound();

            var allPartners = await _db.Partners
                .Include(p => p.PartnerType)
                .Include(p => p.Sales)
                .OrderBy(p => p.CompanyName)
                .ToListAsync();

            ViewBag.AllPartners = allPartners.Select(p => new PartnerViewModel(p)).ToList();

            return View(new PartnerDetailsViewModel
            {
                Partner   = partner,
                ViewModel = new PartnerViewModel(partner),
                Sales     = partner.Sales.OrderByDescending(s => s.SaleDate).ToList()
            });
        }

        public async Task<IActionResult> Create()
        {
            return View(new PartnerEditViewModel
            {
                Types    = await _db.PartnerTypes.OrderBy(t => t.TypeName).ToListAsync(),
                Products = await _db.Products.OrderBy(p => p.Article).ToListAsync()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PartnerEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Types    = await _db.PartnerTypes.OrderBy(t => t.TypeName).ToListAsync();
                vm.Products = await _db.Products.OrderBy(p => p.Article).ToListAsync();
                return View(vm);
            }
            try
            {
                _db.Partners.Add(vm.Partner);
                await _db.SaveChangesAsync();

                if (vm.SaleProductId.HasValue && vm.SaleQuantity.HasValue && vm.SaleDate.HasValue)
                {
                    _db.PartnerSales.Add(new PartnerSale
                    {
                        PartnerId = vm.Partner.Id,
                        ProductId = vm.SaleProductId.Value,
                        Quantity  = vm.SaleQuantity.Value,
                        SaleDate  = DateTime.SpecifyKind(vm.SaleDate.Value.Date, DateTimeKind.Utc)
                    });
                    await _db.SaveChangesAsync();
                }

                _log.LogInformation("Создан партнёр: {Name}", vm.Partner.CompanyName);
                TempData["Success"] = $"Партнёр «{vm.Partner.CompanyName}» успешно добавлен.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _log.LogError("Ошибка создания партнёра: {Err}", ex.Message);
                ModelState.AddModelError("", "Ошибка при сохранении: " + ex.Message);
                vm.Types    = await _db.PartnerTypes.OrderBy(t => t.TypeName).ToListAsync();
                vm.Products = await _db.Products.OrderBy(p => p.Article).ToListAsync();
                return View(vm);
            }
        }
        public async Task<IActionResult> Edit(int id)
        {
            var partner = await _db.Partners.FindAsync(id);
            if (partner == null) return NotFound();

            return View(new PartnerEditViewModel
            {
                Partner  = partner,
                Types    = await _db.PartnerTypes.OrderBy(t => t.TypeName).ToListAsync(),
                Products = await _db.Products.OrderBy(p => p.Article).ToListAsync()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PartnerEditViewModel vm)
        {
            if (id != vm.Partner.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                vm.Types    = await _db.PartnerTypes.OrderBy(t => t.TypeName).ToListAsync();
                vm.Products = await _db.Products.OrderBy(p => p.Article).ToListAsync();
                return View(vm);
            }
            try
            {
                _db.Partners.Update(vm.Partner);

                if (vm.SaleProductId.HasValue && vm.SaleQuantity.HasValue && vm.SaleDate.HasValue)
                {
                    _db.PartnerSales.Add(new PartnerSale
                    {
                        PartnerId = vm.Partner.Id,
                        ProductId = vm.SaleProductId.Value,
                        Quantity  = vm.SaleQuantity.Value,
                        SaleDate  = DateTime.SpecifyKind(vm.SaleDate.Value.Date, DateTimeKind.Utc)
                    });
                }

                await _db.SaveChangesAsync();
                _log.LogInformation("Обновлён партнёр Id={Id}", id);
                TempData["Success"] = $"Данные партнёра «{vm.Partner.CompanyName}» обновлены.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _log.LogError("Ошибка обновления Id={Id}: {Err}", id, ex.Message);
                ModelState.AddModelError("", "Ошибка при сохранении: " + ex.Message);
                vm.Types    = await _db.PartnerTypes.OrderBy(t => t.TypeName).ToListAsync();
                vm.Products = await _db.Products.OrderBy(p => p.Article).ToListAsync();
                return View(vm);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var partner = await _db.Partners
                .Include(p => p.PartnerType)
                .Include(p => p.Sales)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (partner == null) return NotFound();
            return View(new PartnerViewModel(partner));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var partner = await _db.Partners.FindAsync(id);
            if (partner != null)
            {
                _db.Partners.Remove(partner);
                await _db.SaveChangesAsync();
                _log.LogInformation("Удалён партнёр Id={Id}", id);
                TempData["Success"] = "Партнёр удалён.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
