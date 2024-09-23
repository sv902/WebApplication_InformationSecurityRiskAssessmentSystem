using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication_InformationSecurityRiskAssessmentSystem.Models
{
    public class AverageRiskPerAsset
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Актив")]
        public int AssetId { get; set; }

        [Required]
        [Display(Name = "Середній ризик")]
        public decimal AverageRisk { get; set; }

        [Required]
        [Display(Name = "Якісна оцінка ризику")]
        public string? RiskRatingQualitative { get; set; }


        [ForeignKey("AssetId")]
        public virtual Asset? Asset { get; set; }
    }
}
