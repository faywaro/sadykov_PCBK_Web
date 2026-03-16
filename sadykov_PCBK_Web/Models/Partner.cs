using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sadykov_PCBK_Web.Models
{
    [Table("partners", Schema = "app")]
    public class Partner
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите тип партнёра")]
        [Column("type_id")]
        [Display(Name = "Тип партнёра")]
        public int TypeId { get; set; }

        [Required(ErrorMessage = "Укажите наименование компании")]
        [MaxLength(255)]
        [Column("company_name")]
        [Display(Name = "Наименование компании")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите юридический адрес")]
        [MaxLength(500)]
        [Column("legal_address")]
        [Display(Name = "Юридический адрес")]
        public string LegalAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите ИНН")]
        [MaxLength(12)]
        [MinLength(10, ErrorMessage = "ИНН должен содержать от 10 до 12 цифр")]
        [Column("inn")]
        [Display(Name = "ИНН")]
        public string Inn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите ФИО директора")]
        [MaxLength(255)]
        [Column("director_name")]
        [Display(Name = "ФИО директора")]
        public string DirectorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите телефон")]
        [MaxLength(20)]
        [Column("phone")]
        [Display(Name = "Телефон")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите email")]
        [MaxLength(255)]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        [Column("email")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Column("rating")]
        [Display(Name = "Рейтинг")]
        [Range(0, int.MaxValue, ErrorMessage = "Рейтинг должен быть ≥ 0")]
        public int Rating { get; set; } = 0;

        [ForeignKey("TypeId")]
        public virtual PartnerType? PartnerType { get; set; }

        public virtual ICollection<PartnerSale> Sales { get; set; } = new List<PartnerSale>();
    }
}
