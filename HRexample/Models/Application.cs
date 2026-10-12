using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [StringLength(50)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(100)]
        public string s3FileKey { get; set; } = string.Empty;

        [ForeignKey("JobRequirement")]
        public Guid JobRequirementId { get; set; }
        [ValidateNever]
        public JobRequirement JobRequirement { get; set; } 

        [ForeignKey("ModelResponse")]
        public Guid? ModelResponseId { get; set; }
        [ValidateNever]
        public ModelResponse? ModelResponse { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
