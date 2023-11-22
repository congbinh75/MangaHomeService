using System.ComponentModel.DataAnnotations.Schema;

namespace MangaHomeService.Models
{
    public class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public User? CreatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }
        public User? UpdatedBy { get; set; }
    }
}
