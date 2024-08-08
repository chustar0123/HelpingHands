namespace Helping_Hands.Models
{
    public class CareVisitPerNurseModel
    {
        public int VisitId { get; set; }
        public DateTime VisitDate { get; set; }
        public string NurseFirstName { get; set; }
        public TimeSpan VisitArriveTime { get; set; }
        public TimeSpan VisitDepartTime { get; set; }
        public TimeSpan ApproximateArriveTime { get; set; }
        public string NurseSurname { get; set; }
        public string PatientFirstName { get; set; }
        public string WoundCondition { get; set; }
        public string Notes { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
     

    }
}
