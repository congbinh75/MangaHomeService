namespace MangaHomeService.Models.FormDatas.User
{
    public class ChangePassword
    {
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? RepeatNewPassword { get; set; }
    }
}
