using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_InformationSecurityRiskAssessmentSystem.Models
{
    public class RiskAssessment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Користувач")]
        public string? UserId { get; set; }

        [Required]
        [Display(Name = "Актив")]
        public int AssetId { get; set; }

        [Required]
        [Display(Name = "Загроза")]
        public int ThreatId { get; set; }

        [Required]
        [Display(Name = "Ризик")]
        [Range(0, 100, ErrorMessage = "Ризик повинен бути від 0 до 100")]
        public decimal Risk { get; set; }


        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey("AssetId")]
        public virtual Asset? Asset { get; set; }

        [ForeignKey("ThreatId")]
        public virtual Threat? Threat { get; set; }
    }
}
