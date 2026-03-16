using sadykov_PCBK_Web.Helpers;
using sadykov_PCBK_Web.Models;

namespace sadykov_PCBK_Web.ViewModels
{
    public class PartnerViewModel
    {
        public int    Id              { get; set; }
        public string CompanyName     { get; set; } = string.Empty;
        public string TypeName        { get; set; } = string.Empty;
        public string DirectorName    { get; set; } = string.Empty;
        public string Phone           { get; set; } = string.Empty;
        public string Email           { get; set; } = string.Empty;
        public int    Rating          { get; set; }
        public int    TotalQuantity   { get; set; }
        public int    Discount        { get; set; }
        public string DiscountDisplay => $"{Discount}%";
        public string CardTitle       => $"{TypeName} | {CompanyName}";

        public PartnerViewModel() { }

        public PartnerViewModel(Partner p)
        {
            Id           = p.Id;
            CompanyName  = p.CompanyName;
            TypeName     = p.PartnerType?.TypeName ?? "";
            DirectorName = p.DirectorName;
            Phone        = p.Phone;
            Email        = p.Email;
            Rating       = p.Rating;
            TotalQuantity = p.Sales.Sum(s => s.Quantity);
            Discount     = DiscountCalculator.CalculateDiscount(TotalQuantity);
        }
    }

    public class PartnersIndexViewModel
    {
        public List<PartnerViewModel> Partners { get; set; } = new();
        public string? SearchTerm { get; set; }
    }

    public class PartnerDetailsViewModel
    {
        public Partner           Partner       { get; set; } = null!;
        public PartnerViewModel  ViewModel     { get; set; } = null!;
        public List<PartnerSale> Sales         { get; set; } = new();
    }

    public class PartnerEditViewModel
    {
        public Partner              Partner      { get; set; } = new();
        public List<PartnerType>    Types        { get; set; } = new();
        public List<Product>        Products     { get; set; } = new();
        public int?      SaleProductId  { get; set; }
        public int?      SaleQuantity   { get; set; }
        public DateTime? SaleDate       { get; set; }
        public string?   ArticleFilter  { get; set; }
    }

    public class SaleAddViewModel
    {
        public int?      PartnerId     { get; set; }
        public int?      ProductId     { get; set; }
        public int?      Quantity      { get; set; }
        public DateTime? SaleDate      { get; set; }
        public string?   ArticleFilter { get; set; }
        public List<Partner> Partners  { get; set; } = new();
        public List<Product> Products  { get; set; } = new();
    }
}
