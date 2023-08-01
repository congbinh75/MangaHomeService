namespace MangaHomeService.Models.FormData
{
    public class GetTitlesByThemeFormData
    {
        public string ThemeId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
