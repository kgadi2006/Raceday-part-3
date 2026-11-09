namespace RaceDay.Mvc.Models
{
    public class ResultRequest
    {
        public int EnrolmentID { get; set; }

        public TimeSpan FinishTime { get; set; }

        public int FinishingPosition { get; set; }
    }
}