namespace MangaHomeService.Utils
{ 
    public class NotApprovedException : Exception
    {
        public NotApprovedException() { }
        public NotApprovedException(string message) : base(message) { }
    }
}