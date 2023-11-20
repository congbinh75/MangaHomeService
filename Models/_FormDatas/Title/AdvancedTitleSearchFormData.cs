namespace MangaHomeService.Models.FormDatas
{
    public class AdvancedTitleSearchFormData
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Artist { get; set; }
        public List<string>? GenreIds { get; set; }
        public List<string>? ThemeIds { get; set; }
        public string OriginalLanguageId { get; set; }
        public List<string>? LanguageIds { get; set; }
        public List<string>? Statuses { get; set; }
        public bool SortByLastest { get; set; }
        public bool SortByHottest { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
