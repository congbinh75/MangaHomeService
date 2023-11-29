namespace MangaHomeService.Models.FormDatas
{
    public class UpdateUser
    {
        public string? Email { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
}
