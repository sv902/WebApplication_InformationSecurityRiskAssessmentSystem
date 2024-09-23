using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_InformationSecurityRiskAssessmentSystem.Models
{
    public class Asset
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле Назва є обов'язковим")]
        [MaxLength(100, ErrorMessage = "Максимальна довжина поля Назви - 100 символів")]
        [Display(Name = "Назва Активу")]
        public string? Name { get; set; }

        [MaxLength(100, ErrorMessage = "Довжина поля Опис загрози не може перевищувати 100 символів")]
        [Display(Name = "Опис Активу")]
        public string? Description { get; set; }

        [MaxLength(50, ErrorMessage = "Довжина поля Власник загрози не може перевищувати 50 символів")]
        [Display(Name = "Власник Активу")]
        public string? Owner { get; set; }


        public virtual List<AssetThreat>? AssetThreats { get; set; }
    }
}
