namespace MangaHomeService.Models
{
    public class TitleOtherNameModel : BaseModel
    {
        public Title Title { get; set; }
        public string OtherName { get; set; }
        public Language Language { get; set; }
    }
}
