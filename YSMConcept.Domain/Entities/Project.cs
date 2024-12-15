using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YSMConcept.Domain.ValueObjects;

namespace YSMConcept.Domain.Entities
{
    public class Project
    {
        [Key]
        [Column("project_id")]
        public Guid ProjectId { get; set; } = Guid.NewGuid();
        [Column("name")]
        public string Name { get; set; } = null!;
        [Column("building_type")]
        public string BuildingType { get; set; } = null!;
        [Column("area")]
        public int Area { get; set; } 
        public Date Date { get; set; } = null!;
        public Address Address { get; set; } = null!;
        [Column("description")]
        public string? Description { get; set; }
        public List<ImageEntity>? CollectionImages { get; set; } = new List<ImageEntity>();
    }
}
