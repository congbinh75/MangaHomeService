namespace MangaHomeService.Utils
{ 
    public class AlreadyApprovedException : Exception
    {
        public AlreadyApprovedException() { }
        public AlreadyApprovedException(string message) : base(message) { }
    }
}