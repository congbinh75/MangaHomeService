namespace MangaHomeService.Utils
{ 
    public class AlreadyRemovedException : Exception
    {
        public AlreadyRemovedException() { }
        public AlreadyRemovedException(string message) : base(message) { }
    }
}