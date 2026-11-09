namespace RaceDay.Mvc.Models
{
    public class OrganizerEnrolment
    {
        public int EnrolmentID { get; set; }
        public int EventID { get; set; }

        public string ParticipantName { get; set; } = string.Empty;
        public string ParticipantEmail { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;
        public string CategoryType { get; set; } = string.Empty;
        public string CategoryValue { get; set; } = string.Empty;

        public DateTime EnrolmentDate { get; set; }

        // Result information
        public int? ResultID { get; set; }
        public TimeSpan? FinishTime { get; set; }
        public int? FinishingPosition { get; set; }
    }
}