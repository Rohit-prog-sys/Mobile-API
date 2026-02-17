using MobileAPI.Models;

namespace MobileAPI.Interface
{
    public interface ITransactionRepository
    {
        Task AddAsync(TransactionEntity txn);
        Task<TransactionEntity?> GetAsync(string txnId);
        Task UpdateStatusAsync(string txnId, string status, string respCode);
    }
}
