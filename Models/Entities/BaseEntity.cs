using System.ComponentModel.DataAnnotations.Schema;

namespace MangaHomeService.Models.Entities
{
    public class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = string.Empty;

        public DateTime CreatedTime { get; set; }

        public string? CreatedById { get; set; }

        [NotMapped]
        public User? CreatedBy { get; set; }

        public DateTime UpdatedTime { get; set; }

        public string? UpdatedById { get; set; }

        [NotMapped]
        public User? UpdatedBy { get; set; }
    }
}
