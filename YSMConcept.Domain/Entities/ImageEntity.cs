using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace YSMConcept.Domain.Entities
{
    public class ImageEntity
    {
        [Key]
        [Column("image_id")]
        public string ImageId { get; set; } = null!;
        [Column("image_url")]
        public string ImageURL { get; set; } = null!;
        [Column("is_main")]
        public bool IsMain { get; set; } = false;
        [Column("project_id")]
        public Guid ProjectId { get; set; }
        [JsonIgnore]
        public Project Project { get; set; } = null!;
    }
}
