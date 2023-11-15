namespace MangaHomeService.Models
{
    public class OtherName : BaseModel
    {
        public string Name { get; set; }
        public Language Language { get; set; }
    }
    public class TitleOtherName : OtherName
    {
        public Title Title { get; set; }
    }

    public class GenreOtherName : OtherName
    {
        public Genre Gerne { get; set; }
    }

    public class ThemeOtherName : OtherName
    {
        public Theme Theme { get; set; }
    }
}
