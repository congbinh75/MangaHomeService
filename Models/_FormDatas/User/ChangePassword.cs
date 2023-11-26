namespace MangaHomeService.Models.FormDatas.User
{
    public class ChangePassword
    {
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }

        public bool Validate()
        {
            if (!string.IsNullOrEmpty(OldPassword) && !string.IsNullOrEmpty(NewPassword))
            {
                return true;
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
