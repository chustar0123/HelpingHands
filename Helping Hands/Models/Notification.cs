namespace Helping_Hands.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public int PatientId { get; set; } // The patient who created the care contract
        public DateTime Timestamp { get; set; }
        public Patient Patient { get; set; }
    }
}
