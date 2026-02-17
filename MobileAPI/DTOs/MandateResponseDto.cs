namespace MobileAPI.DTOs
{
    public class MandateResponseDto
    {
        public string PayerVpa { get; set; }
        public string PayeeVpa { get; set; }
        public decimal Amount { get; set; }
        public string Note { get; set; }
        public string TxnType { get; set; } 
        public string TxnID { get; set; }
    }
}
