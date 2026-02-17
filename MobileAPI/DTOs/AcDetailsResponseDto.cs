namespace MobileAPI.DTOs.Common
{
    public class AcDetailsResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public string AccountNumber { get; set; }
        public string CustomerName { get; set; }
        public string AcType { get; set; }
        public string IFSC { get; set; }
        public string CustomerNo { get; set; }
        public string CardNo { get; set; }
        public string Expiry { get; set; }
    }

  

}
