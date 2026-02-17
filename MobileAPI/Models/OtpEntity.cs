namespace MobileAPI.Models
{

    public class OtpEntity
    {
        public int Id { get; set; }
        public string MobileNumber { get; set; }
        public string OTP { get; set; }
        public DateTime GentTime { get; set; }
        public DateTime ValTime { get; set; }
        public string Status { get; set; }
        public string IPAdd { get; set; }
    }

}
