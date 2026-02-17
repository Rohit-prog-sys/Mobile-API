using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using MobileAPI.Interface;
using MobileAPI.Models;



namespace MobileAPI.Repositories
{
    using System.Data;
    using Dapper;
    using Microsoft.Data.SqlClient;

    public class OTPRepository : IOTPRepository
    {
        private readonly string _connectionString;

        public OTPRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Default");
        }

        private IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);

        // ✅ Insert OTP
        public async Task InsertAsync(OtpEntity otp)
        {
            const string sql = @"
    INSERT INTO MobileOTP
    (MobileNumber,OTP,GentTime,ValTime,Status,IPAdd)
    VALUES
    (@MobileNumber,@OTP,@GentTime,@ValTime,@Status,@IPAdd)";

            try
            {
                using var con = CreateConnection();

                await con.ExecuteAsync(sql, otp);
            }
            catch (SqlException ex)
            {
                // SQL specific errors
                // example: timeout, deadlock, connection fail
                throw new Exception("Database error while inserting OTP", ex);
            }
            catch (Exception ex)
            {
                // unexpected errors
                throw new Exception("Unexpected error while saving OTP", ex);
            }
        }


        // ✅ Validate OTP
        public async Task<OtpEntity?> GetValidOtpAsync(string mobile)
        {
            var sql = @"
        SELECT TOP 1 *
        FROM MobileOTP
        WHERE MobileNumber=@mobile
        ORDER BY Id GenTime DESC";

            using var con = CreateConnection();
            return await con.QueryFirstOrDefaultAsync<OtpEntity>(sql, new { mobile});
        }

        // ✅ Mark used
        public async Task MarkUsedAsync(int id, string status)
        {
            const string sql = @"
        UPDATE MobileOTP 
        SET Status = @Status
        WHERE Id = @Id";

            try
            {
                using var con = CreateConnection();

                await con.ExecuteAsync(sql, new
                {
                    Id = id,
                    Status = status
                });
            }
            catch (SqlException ex)
            {
                throw new Exception($"Database error while updating OTP status. Id={id}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error while updating OTP status. Id={id}", ex);
            }
        }


    }


}
