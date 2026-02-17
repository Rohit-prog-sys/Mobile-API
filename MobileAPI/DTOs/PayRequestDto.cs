using MobileAPI.DTOs.Common;

namespace MobileAPI.DTOs
{
    public class PayRequestDto
    {
        public string PayerVpa { get; set; }
        public string PayeeVpa { get; set; }
        public string PayerAcNo { get; set; }
        public decimal Amount { get; set; }
        public string Note { get; set; }
        public string TxnType { get; set; } 
        public string TxnID { get; set; }
        public string EntryMode { get; set; }
        public DeviceInfoDto Device { get; set; }
    }
}
