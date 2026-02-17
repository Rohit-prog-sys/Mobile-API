using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using MobileAPI.Interface;
using MobileAPI.Models;



namespace MobileAPI.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly string _connectionString;

        public TransactionRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Default");
        }

        private IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);

        // ✅ Insert
        public async Task AddAsync(TransactionEntity txn)
        {
            var sql = @"
            INSERT INTO UpiTransactions
            (TxnId,PayerVpa,PayeeVpa,Amount,Status,CreatedAt)
            VALUES
            (@TxnId,@PayerVpa,@PayeeVpa,@Amount,@Status,GETDATE())";

            using var con = CreateConnection();
            await con.ExecuteAsync(sql, txn);
        }

        // ✅ Select
        public async Task<TransactionEntity?> GetAsync(string txnId)
        {
            var sql = "SELECT * FROM UpiTransactions WHERE TxnId = @txnId";

            using var con = CreateConnection();
            return await con.QueryFirstOrDefaultAsync<TransactionEntity>(sql, new { txnId });
        }

       

        // ✅ Update
        public async Task UpdateStatusAsync(string txnId, string status, string respCode)
        {
            var sql = @"
            UPDATE UpiTransactions
            SET Status=@status,
                ResponseCode=@respCode,
                UpdatedAt=GETDATE()
            WHERE TxnId=@txnId";

            using var con = CreateConnection();
            await con.ExecuteAsync(sql, new { txnId, status, respCode });
        }
    }

}
