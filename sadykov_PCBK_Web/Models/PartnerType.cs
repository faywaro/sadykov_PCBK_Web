using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sadykov_PCBK_Web.Models
{
    [Table("partner_types", Schema = "app")]
    public class PartnerType
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("type_name")]
        [Display(Name = "Тип партнёра")]
        public string TypeName { get; set; } = string.Empty;

        public virtual ICollection<Partner> Partners { get; set; } = new List<Partner>();
    }
}
