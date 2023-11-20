namespace MangaHomeService.Models.FormDatas
{
    public class GetTitlesByThemeFormData
    {
        public string ThemeId { get; set; }
        public string PageNumber { get; set; }
        public string PageSize { get; set; }
    }
}
