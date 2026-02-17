namespace MobileAPI.Models
{

    public class TransactionEntity
    {
        public int Id { get; set; }
        public string TxnId { get; set; }
        public string PayerVpa { get; set; }
        public string PayeeVpa { get; set; }
        public decimal Amount { get; set; }
        public decimal CustRef { get; set; }
        public string Status { get; set; }
        public string ResponseCode { get; set; }
    }


}
