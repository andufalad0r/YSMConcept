using System.ComponentModel.DataAnnotations;

namespace YSMConcept.Application.DTOs.AuthDTOs
{
    public class LoginDTO
    {
        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        public string Password { get; set; } = null!;
    }

}
