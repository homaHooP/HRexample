using System.ComponentModel.DataAnnotations;

namespace HRexample.Models
{
    public class JobRequirement
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Вкажіть посаду")]
        [StringLength(500)]
        [Display(Name = "Посада")]
        public string Position { get; set; } = string.Empty;
        public string s3FileKey { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
