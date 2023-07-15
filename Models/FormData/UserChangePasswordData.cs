namespace MangaHomeService.Models.FormData
{
    public class UserChangePasswordData
    {
        public string? oldPassword { get; set; }
        public string? newPassword { get; set; }
        public string? repeatNewPassword { get; set; }
    }
}
