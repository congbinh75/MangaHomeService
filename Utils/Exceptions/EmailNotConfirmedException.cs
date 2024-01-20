namespace MangaHomeService.Utils
{ 
    public class EmailNotConfirmedException : Exception
    {
        public EmailNotConfirmedException() { }
        public EmailNotConfirmedException(string message) : base(message) { }
    }
}