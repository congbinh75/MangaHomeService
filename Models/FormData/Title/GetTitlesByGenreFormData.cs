namespace MangaHomeService.Models.FormData
{
    public class GetTitlesByGenreFormData
    {
        public string GenreId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
