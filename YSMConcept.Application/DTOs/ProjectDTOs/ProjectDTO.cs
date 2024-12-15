using YSMConcept.Application.DTOs.ImageDTOs;
using YSMConcept.Domain.ValueObjects;

namespace YSMConcept.Application.DTOs.ProjectDTOs
{
    public class ProjectDTO
    {
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = null!;
        public string BuildingType { get; set; } = null!;
        public int Area { get; set; } 
        public Date Date { get; set; } = null!;
        public Address Address { get; set; } = null!;
        public string? Description { get; set; }
        public List<ImageDTO>? CollectionImages { get; set; } = new List<ImageDTO>();
    }
}
