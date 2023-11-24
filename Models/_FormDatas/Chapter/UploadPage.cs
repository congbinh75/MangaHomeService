namespace MangaHomeService.Models.FormDatas.Chapter
{
    public class UploadPage
    {
        public string ChapterId { get; set; }
        public string Number { get; set; }
        public IFormFile File { get; set; }
    }
}
