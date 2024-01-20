namespace MangaHomeService.Utils
{ 
    public class AlreadyReviewedException : Exception
    {
        public AlreadyReviewedException() { }
        public AlreadyReviewedException(string message) : base(message) { }
    }
}