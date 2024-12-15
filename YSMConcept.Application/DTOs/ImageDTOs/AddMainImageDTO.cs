using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace YSMConcept.Application.DTOs.ImageDTOs
{
    public class AddMainImageDTO
    {
        [Required]
        public IFormFile MainImage { get; set; } = null!;
    }
}
