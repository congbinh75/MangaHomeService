using static MangaHomeService.Utils.Enums;

namespace MangaHomeService.Models
{
    public class Title : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Artwork { get; set; }
        public string Author { get; set; }
        public string Artist { get; set; }
        public TitleStatus Status { get; set; }
        public List<Chapter> Chapters { get; set; }
        public List<Comment> Comments { get; set; }

        public Title() { }
        public Title(string name, string description, string artwork, string author, string artist, int status, 
            List<Chapter> chapters, List<Comment> comments) 
        {
            Name = name;
            Description = description;
            Artwork = artwork;
            Author = author;
            Artist = artist;
            Status = (TitleStatus)status;
            Chapters = chapters;
            Comments = comments;
        }
    }
}
