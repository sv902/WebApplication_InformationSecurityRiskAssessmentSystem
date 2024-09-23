using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_InformationSecurityRiskAssessmentSystem.Models
{
    public class Threat
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле Назва є обов'язковим")]
        [MaxLength(100, ErrorMessage = "Максимальна довжина поля Назви - 100 символів")]
        [Display(Name = "Назва Загрози")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Поле опис є обов'язковим")]
        [MaxLength(100, ErrorMessage = "Максимальна довжина поля опису - 100 символів")]
        [Display(Name = "Опис загрози")]
        public string? Description { get; set; }


        public virtual List<AssetThreat>? AssetThreats { get; set; }
    }
}
