using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using YSMConcept.Domain.ValueObjects;

namespace YSMConcept.Application.DTOs.ProjectDTOs
{
    public class CreateProjectDTO
    {
        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        public string Name { get; set; } = null!;
        [Required]
        [MinLength(0)]
        [MaxLength(18)]
        public string BuildingType { get; set; } = null!;
        [Required]
        [Range(0, 50000)]
        public int Area { get; set; } // in m^2
        public Date Date { get; set; } = null!;
        public Address Address { get; set; } = null!;
        [Required]
        [MinLength(0)]
        [MaxLength(500)]
        public string? Description { get; set; }
        public IFormFile? MainImage { get; set; }
        public List<IFormFile>? CollectionImages { get; set; } = new List<IFormFile>();
    }
}
