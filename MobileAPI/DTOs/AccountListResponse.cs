namespace MobileAPI.DTOs.Common
{
    public class AccountListResponseDto
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
         public List<Account> Accounts { get; set; }
    }

    public class Account
    {
        public string AccountNumber { get; set; }
        public string CustomerName { get; set; }
        public string Actype { get; set; }
        public string IFSC { get; set; }
       
    }

}
