using MangaHomeService.Models;
using static MangaHomeService.Utils.Enums;

namespace MangaHomeService.Models
{
    public class Title : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Artwork { get; set; }
        public Person Author { get; set; }
        public Person Artist { get; set; }
        public List<Demographic> Demographics { get; set; }
        public TitleStatus Status { get; set; }
        public double Rating { get; set; }
        public int RatingVotes { get; set; }
        public int Views { get; set; }
        public int Bookmarks { get; set; }
        public List<OtherName> OtherNames { get; set; }
        public Language OriginalLanguage { get; set; }
        public List<Genre> Gernes { get; set; }
        public List<Theme> Themes { get; set; }
        public List<Chapter> Chapters { get; set; }
        public List<Comment> Comments { get; set; }
        public bool IsAprroved { get; set; }
    }
}
