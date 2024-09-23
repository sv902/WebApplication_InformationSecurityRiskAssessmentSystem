using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication_InformationSecurityRiskAssessmentSystem.Models
{
    public class AverageRiskPerAssetThreat
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Актив")]
        public int AssetId { get; set; }

        [Required]
        [Display(Name = "Загроза")]
        public int ThreatId { get; set; }

        [Required]
        [Display(Name = "Ризик")]
        public decimal AverageRisk { get; set; }

        [Required]
        [Display(Name = "Якісна оцінка ризику")]
        public string? RiskRatingQualitative { get; set; }

        [Required]
        [Display(Name = "Дата оцінювання")]
        public DateTime AssessmentDate { get; set; }



        [ForeignKey("AssetId")]
        public virtual Asset? Asset { get; set; }

        [ForeignKey("ThreatId")]
        public virtual Threat? Threat { get; set; }

    }
}
