using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sadykov_PCBK_Web.Models
{
    [Table("products", Schema = "app")]
    public class Product
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите артикул")]
        [MaxLength(50)]
        [Column("article")]
        [Display(Name = "Артикул")]
        public string Article { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите наименование")]
        [MaxLength(255)]
        [Column("product_name")]
        [Display(Name = "Наименование")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите тип продукции")]
        [MaxLength(100)]
        [Column("product_type")]
        [Display(Name = "Тип продукции")]
        public string ProductType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите минимальную цену")]
        [Column("min_price")]
        [Display(Name = "Мин. цена (руб.)")]
        [Range(0, double.MaxValue, ErrorMessage = "Цена должна быть ≥ 0")]
        public decimal MinPrice { get; set; }

        public virtual ICollection<PartnerSale> Sales { get; set; } = new List<PartnerSale>();
    }

    [Table("partner_sales", Schema = "app")]
    public class PartnerSale
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("partner_id")]
        public int PartnerId { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Укажите количество")]
        [Column("quantity")]
        [Display(Name = "Количество")]
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть > 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Укажите дату")]
        [Column("sale_date")]
        [Display(Name = "Дата реализации")]
        [DataType(DataType.Date)]
        public DateTime SaleDate { get; set; } = DateTime.Today;

        [ForeignKey("PartnerId")]
        public virtual Partner? Partner { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}
