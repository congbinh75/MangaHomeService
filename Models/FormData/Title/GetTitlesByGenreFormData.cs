namespace MangaHomeService.Models.FormData
{
    public class GetTitlesByGenreFormData
    {
        public string GenreId { get; set; }
        public string PageNumber { get; set; }
        public string PageSize { get; set; }
    }
}
