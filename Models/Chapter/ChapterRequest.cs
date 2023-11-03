﻿namespace MangaHomeService.Models
{
    public class ChapterRequest : BaseModel
    {
        public Chapter Chapter { get; set; }
        public User SubmitUser { get; set; }
        public Group Group { get; set; }
        public string Note { get; set; }
        public User ReviewUser { get; set; }
        public bool IsApproved { get; set; }
        public bool IsReviewed { get; set; }
    }
}
