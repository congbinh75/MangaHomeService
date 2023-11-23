using MangaHomeService.Utils;
using static MangaHomeService.Utils.Enums;

namespace MangaHomeService.Models
{
    public class Title : BaseEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; } = string.Empty;
        public string? Artwork { get; set; } = string.Empty;
        public required ICollection<Person> Authors { get; set; } = [];
        public required ICollection<Person> Artists { get; set; } = [];
        public ICollection<Tag> Demographics { get; set; } = [];
        public required TitleStatus Status { get; set; } = 0;
        public required double Rating { get; set; } = 0;
        public required int RatingVotes { get; set; } = 0;
        public required int Views { get; set; } = 0;
        public required int Bookmarks { get; set; } = 0;
        public ICollection<OtherName> OtherNames { get; set; } = [];
        public Language? OriginalLanguage { get; set; }
        public required ICollection<Tag> Gernes { get; set; } = [];
        public required ICollection<Tag> Themes { get; set; } = [];
        public required ICollection<Chapter> Chapters { get; set; } = [];
        public required ICollection<Comment> Comments { get; set; } = [];
        public required bool IsAprroved { get; set; }

        public void CheckUploadConditions()
        {
            if (!IsAprroved)
            {
                throw new NotApprovedException(Name ?? "");
            }
        }
    }
}
