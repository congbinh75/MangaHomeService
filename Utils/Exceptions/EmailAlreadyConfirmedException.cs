namespace MangaHomeService.Utils
{ 
    public class EmailAlreadyConfirmedException : Exception
    {
        public EmailAlreadyConfirmedException() { }
        public EmailAlreadyConfirmedException(string message) : base(message) { }
    }
}