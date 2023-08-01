namespace MangaHomeService.Models
{
    public class TitleOtherName : BaseModel
    {
        public Title Title { get; set; }
        public string OtherName { get; set; }
        public Language Language { get; set; }

        public TitleOtherName() { }
        public TitleOtherName(Title title, string otherName, Language language) 
        {
            Title = title;
            OtherName = otherName;
            Language = language;
        }
    }
}
