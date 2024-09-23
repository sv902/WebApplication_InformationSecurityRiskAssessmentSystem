using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebApplication_InformationSecurityRiskAssessmentSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        [Display(Name ="Ім'я користувача")]
        public string? FirstName { get; set; }
            
        [MaxLength(100)]
        [Display(Name = "Прізвище користувача")]
        public string? LastName { get; set; }
    }
}
