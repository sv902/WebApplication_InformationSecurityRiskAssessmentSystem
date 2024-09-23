using System.ComponentModel.DataAnnotations;
namespace WebApplication_InformationSecurityRiskAssessmentSystem.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }
        //
        [Required(ErrorMessage = "Поле Назва є обов'язковим")]
        [MaxLength(100, ErrorMessage = "Максимальна довжина поля Назви - 100 символів")]
        [Display(Name = "Ім`я")]
        public string? Name { get; set; }
        //
        [Required(ErrorMessage = "Телефон є обов'язковим")]
        [MaxLength(10, ErrorMessage = "Номер має мати 10 цифр")]
        [MinLength(10, ErrorMessage = "Номер має мати 10 цифр")]
        [Display(Name = "Номер")]
        public string? Phone { get; set; }
        //
        [Required(ErrorMessage = "Поле Email є обов'язковим")]
        [EmailAddress(ErrorMessage = "Неправильний формат")]
        [Display(Name = "Пошта")]
        public string? Email { get; set; }
        //
        [Required(ErrorMessage = "Поле є обов'язковим")]
        [MaxLength(100, ErrorMessage = "Довжина поля Опис загрози не може перевищувати 100 символів")]
        [Display(Name = "Тема")]
        public string? Description { get; set; }
        //
        [Required(ErrorMessage = "Поле є обов'язковим")]
        [MaxLength(500, ErrorMessage = "Довжина поля Опис загрози не може перевищувати 500 символів")]
        [Display(Name = "Повідомлення")]
        public string? Message { get; set; }

        [Display(Name = "Час")]
        public DateTime DateAdded { get; set; }




    }
}
