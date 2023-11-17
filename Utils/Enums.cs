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

        public enum TagType
        {
            Gerne = 0,
            Theme = 1,
            Demographic = 2
        }
    }
}
