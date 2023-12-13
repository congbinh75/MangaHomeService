namespace MangaHomeService.Utils
{
    public static class Enums
    {
        public enum Role
        {
            Admin = 0,
            Moderator = 1,
            User = 2
        }

        public enum GroupRole
        {
            Leader = 0,
            Moderator = 1,
            Member = 2
        }

        public enum TitleStatus
        {
            NotYetReleased = 0,
            OnGoing = 1,
            Completed = 2,
            Abandoned = 3
        }

        public enum RequestType
        {
            Group = 0,
            Member = 1,
            Title = 2,
            Chapter = 3,
            Author = 4,
            Artist = 5
        }

        public enum ReportType
        {
            Group = 0,
            Title = 1,
            Chapter = 2,
            User = 3
        }
    }
}
