using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_InformationSecurityRiskAssessmentSystem.Models
{
    public class AssetThreat
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Актив")]
        public int AssetId { get; set; }

        [Required]
        [Display(Name = "Загроза")]
        public int ThreatId { get; set; }
      
        [ForeignKey("AssetId")]
        public virtual Asset? Asset { get; set; }
        
        [ForeignKey("ThreatId")]
        public virtual Threat? Threat { get; set; }
    }
}
