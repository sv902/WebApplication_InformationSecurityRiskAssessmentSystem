using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_InformationSecurityRiskAssessmentSystem.Models
{
    public class ThreatAssessment
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

        [Required(ErrorMessage = "Поле Ймовірність є обов'язковим")]
        [Range(0, 10, ErrorMessage = "Ймовірність повинна бути в межах від 0 до 10.")]
        [Display(Name = "Ймовірність")]
        public decimal Likelihood { get; set; }

        [Required(ErrorMessage = "Поле Наслідки є обов'язковим")]
        [Range(0, 15, ErrorMessage = "Наслідки повинні бути в межах від 0 до 15.")]
        [Display(Name = "Наслідки")]
        public decimal Consequences { get; set; }
              
        [Display(Name = "Дата оцінювання")]
        [DataType(DataType.Date)]
        public DateTime AssessmentDate { get; set; } = DateTime.Now;



        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey("AssetId")]
        public virtual Asset? Asset { get; set; }

        [ForeignKey("ThreatId")]
        public virtual Threat? Threat { get; set; }
    }

}
