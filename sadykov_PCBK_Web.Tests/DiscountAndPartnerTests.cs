using Xunit;
using Microsoft.EntityFrameworkCore;
using sadykov_PCBK_Web.Data;
using sadykov_PCBK_Web.Models;
using sadykov_PCBK_Web.Helpers;
using sadykov_PCBK_Web.ViewModels;

namespace sadykov_PCBK_Web.Tests
{
    public class DiscountCalculatorTests
    {
        [Theory]
        [InlineData(0,       0)]
        [InlineData(9999,    0)]
        [InlineData(10000,   5)]
        [InlineData(30000,   5)]
        [InlineData(49999,   5)]
        [InlineData(50000,  10)]
        [InlineData(100000, 10)]
        [InlineData(299999, 10)]
        [InlineData(300000, 15)]
        [InlineData(999999, 15)]
        public void CalculateDiscount_ReturnsCorrectValue(int qty, int expected)
        {
            Assert.Equal(expected, DiscountCalculator.CalculateDiscount(qty));
        }

        [Fact]
        public void CalculateDiscount_ZeroQuantity_ReturnsZero()
            => Assert.Equal(0, DiscountCalculator.CalculateDiscount(0));

        [Fact]
        public void CalculateDiscount_MaxInt_Returns15()
            => Assert.Equal(15, DiscountCalculator.CalculateDiscount(int.MaxValue));
    }

    public class PartnerRepositoryTests
    {
        private ApplicationDbContext CreateCtx()
        {
            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var ctx = new ApplicationDbContext(opts);
            ctx.PartnerTypes.Add(new PartnerType { Id = 1, TypeName = "ООО" });
            ctx.SaveChanges();
            return ctx;
        }

        private Partner MakePartner(ApplicationDbContext ctx)
        {
            var p = new Partner
            {
                TypeId       = 1,
                CompanyName  = "УралУпаковка",
                LegalAddress = "г. Екатеринбург, ул. Малышева, 101",
                Inn          = "6670123456",
                DirectorName = "Козлов Д.А.",
                Phone        = "+73432345678",
                Email        = "info@ural.ru"
            };
            ctx.Partners.Add(p);
            ctx.SaveChanges();
            return p;
        }

        [Fact]
        public void Add_Partner_PersistsToDatabase()
        {
            var ctx = CreateCtx();
            MakePartner(ctx);
            Assert.Equal(1, ctx.Partners.Count());
        }

        [Fact]
        public void Find_Partner_ById_ReturnsCorrectPartner()
        {
            var ctx = CreateCtx();
            var p   = MakePartner(ctx);
            var found = ctx.Partners.Find(p.Id);
            Assert.NotNull(found);
            Assert.Equal("УралУпаковка", found!.CompanyName);
        }

        [Fact]
        public void Find_Partner_InvalidId_ReturnsNull()
        {
            var ctx = CreateCtx();
            Assert.Null(ctx.Partners.Find(999));
        }

        [Fact]
        public void Update_Partner_ChangesCompanyName()
        {
            var ctx = CreateCtx();
            var p   = MakePartner(ctx);
            p.CompanyName = "НовоеНазвание";
            ctx.Partners.Update(p);
            ctx.SaveChanges();
            Assert.Equal("НовоеНазвание", ctx.Partners.Find(p.Id)!.CompanyName);
        }

        [Fact]
        public void Delete_Partner_RemovesFromDatabase()
        {
            var ctx = CreateCtx();
            var p   = MakePartner(ctx);
            ctx.Partners.Remove(p);
            ctx.SaveChanges();
            Assert.Equal(0, ctx.Partners.Count());
        }

        [Fact]
        public void TotalQuantity_CalculatedCorrectly()
        {
            var ctx = CreateCtx();
            var p   = MakePartner(ctx);
            ctx.Products.Add(new Product
            {
                Id = 1, Article = "PCBK-001",
                ProductName = "Гофрокартон Т-23",
                ProductType = "Гофрокартон", MinPrice = 250
            });
            ctx.SaveChanges();
            ctx.PartnerSales.AddRange(
                new PartnerSale { PartnerId = p.Id, ProductId = 1, Quantity = 5000,  SaleDate = DateTime.Today },
                new PartnerSale { PartnerId = p.Id, ProductId = 1, Quantity = 8000,  SaleDate = DateTime.Today },
                new PartnerSale { PartnerId = p.Id, ProductId = 1, Quantity = 12000, SaleDate = DateTime.Today }
            );
            ctx.SaveChanges();

            var total = ctx.PartnerSales
                .Where(s => s.PartnerId == p.Id)
                .Sum(s => s.Quantity);
            Assert.Equal(25000, total);
        }

        [Fact]
        public void PartnerViewModel_DiscountBasedOnTotalQuantity()
        {
            var ctx = CreateCtx();
            var p   = MakePartner(ctx);
            ctx.Products.Add(new Product
            {
                Id = 2, Article = "PCBK-002",
                ProductName = "Тест-лайнер 125",
                ProductType = "Бумага", MinPrice = 180
            });
            ctx.SaveChanges();
            ctx.PartnerSales.Add(new PartnerSale
            {
                PartnerId = p.Id, ProductId = 2,
                Quantity = 300000, SaleDate = DateTime.Today
            });
            ctx.SaveChanges();

            var reloaded = ctx.Partners
                .Include(x => x.PartnerType)
                .Include(x => x.Sales)
                .First(x => x.Id == p.Id);

            var vm = new PartnerViewModel(reloaded);
            Assert.Equal(15, vm.Discount);
            Assert.Equal("15%", vm.DiscountDisplay);
        }

        [Fact]
        public void Sale_CascadeDelete_RemovesWithPartner()
        {
            var ctx = CreateCtx();
            var p   = MakePartner(ctx);
            ctx.Products.Add(new Product
            {
                Id = 3, Article = "PCBK-003",
                ProductName = "Крафт-бумага 80",
                ProductType = "Бумага", MinPrice = 120
            });
            ctx.SaveChanges();
            ctx.PartnerSales.Add(new PartnerSale
            {
                PartnerId = p.Id, ProductId = 3,
                Quantity = 1000, SaleDate = DateTime.Today
            });
            ctx.SaveChanges();
            Assert.Equal(1, ctx.PartnerSales.Count());

            ctx.Partners.Remove(p);
            ctx.SaveChanges();
            Assert.Equal(0, ctx.PartnerSales.Count());
        }
    }

    public class PartnerViewModelTests
    {
        private static Partner MakePartner(int totalQty)
        {
            var p = new Partner
            {
                Id = 1, CompanyName = "ТестПартнёр",
                DirectorName = "Иванов И.И.", Phone = "+7",
                Email = "test@test.ru", Inn = "1234567890",
                LegalAddress = "г. Пермь", TypeId = 1,
                PartnerType = new PartnerType { TypeName = "ООО" }
            };
            if (totalQty > 0)
                p.Sales.Add(new PartnerSale
                {
                    Quantity = totalQty, SaleDate = DateTime.Today,
                    Product = new Product { ProductName = "Тест", Article = "T", ProductType = "Т", MinPrice = 1 }
                });
            return p;
        }

        [Fact]
        public void CardTitle_FormattedCorrectly()
        {
            var vm = new PartnerViewModel(MakePartner(0));
            Assert.Equal("ООО | ТестПартнёр", vm.CardTitle);
        }

        [Theory]
        [InlineData(0,       "0%")]
        [InlineData(9999,    "0%")]
        [InlineData(10000,   "5%")]
        [InlineData(50000,  "10%")]
        [InlineData(300000, "15%")]
        public void DiscountDisplay_CorrectForQuantity(int qty, string expected)
        {
            var vm = new PartnerViewModel(MakePartner(qty));
            Assert.Equal(expected, vm.DiscountDisplay);
        }
    }
}
