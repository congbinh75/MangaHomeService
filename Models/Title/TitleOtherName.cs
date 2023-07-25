namespace MangaHomeService.Models
{
    public class TitleOtherName : BaseModel
    {
        public Title Title { get; set; }
        public string OtherName { get; set; }

        public TitleOtherName() { }
        public TitleOtherName(Title title, string otherName) 
        {
            Title = title;
            OtherName = otherName;
        }
    }
}
