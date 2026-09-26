using System.ComponentModel.DataAnnotations;

namespace HRexample.Models
{
    public class Application
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Вкажіть своє повне ім'я")]
        [StringLength(50)]
        [Display(Name = "Ім'я апліканта")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть свою email")]
        [EmailAddress(ErrorMessage = "Вкажіть коректну email адресу")]
        [StringLength(20)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(100)]
        public string s3FileKey { get; set; } = $"{Guid.NewGuid()}";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
