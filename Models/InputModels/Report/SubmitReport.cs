using System.ComponentModel.DataAnnotations;

namespace MangaHomeService.Models.InputModels
{
    public class SubmitReport
    {

        [Required]
        public required string Reason { get; set; }

        [Required]
        [MaxLength(256)]
        public required string Note { get; set; }

    }

    public class GroupReportData : SubmitReport
    {
        [Required]
        public required string GroupId { get; set; }
    }

    public class TitleReportData : SubmitReport
    {
        [Required]
        public required string TitleId { get; set; }
    }

    public class ChapterReportData : SubmitReport
    {
        [Required]
        public required string ChapterId { get; set; }
    }

    public class UserReportData : SubmitReport
    {
        [Required]
        public required string UserId { get; set; }
    }
}
