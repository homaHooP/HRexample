using System.ComponentModel.DataAnnotations;

namespace HRexample.Models
{
    public class JobRequirement
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Вкажіть ім'я файла")]
        [StringLength(500)]
        [Display(Name = "Ім'я")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть вимоги для посади")]
        [StringLength(500)]
        [Display(Name = "Вимоги")]
        public string RequiredSkills { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
