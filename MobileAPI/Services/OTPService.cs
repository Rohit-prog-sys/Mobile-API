using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using MobileAPI.Interface;
using MobileAPI.Models;

public class OTPService
{
    private readonly IOTPRepository _repo;
    private readonly IConfiguration _config;
    private readonly HttpClient _http;

    public OTPService(
        IOTPRepository repo,
        IConfiguration config,
        HttpClient http)
    {
        _repo = repo;
        _config = config;
        _http = http;
    }

    public async Task GenerateAsync(string mobile, string ip)
    {
        var otp = GenerateSecureOtp();

        var entity = new OtpEntity
        {
            MobileNumber = mobile,
            OTP = otp,
            GentTime = DateTime.Now,
            ValTime = DateTime.Now.AddMinutes(3),
            Status = "GENERATED",
            IPAdd = ip
        };

        await _repo.InsertAsync(entity);

        await SendSmsAsync(otp, mobile);
    }

    public async Task<(bool Success, string Message)> VerifyOtpAsync(string mobile, string otp)
    {
        var data = await _repo.GetValidOtpAsync(mobile);
        //swati
        if (data == null)
        {
            return (false, "OTP not found");
        }
            
        if(otp != data.OTP)
        {
            return (false, "Invalid OTP");
        }

        if (DateTime.Now > data.ValTime)
        {
            await _repo.MarkUsedAsync(data.Id, "EXPIRED");
            return (false, "OTP Expired");
        }

        await _repo.MarkUsedAsync(data.Id, "VALIDATED");

        return (true, "OTP Verified Successfully");
    }

    private static string GenerateSecureOtp()
    {
        var bytes = new byte[4];
        RandomNumberGenerator.Fill(bytes);

        int value = BitConverter.ToInt32(bytes, 0) & 0x7FFFFFFF;

        return (value % 900000 + 100000).ToString(); // 6 digit
    }

    private async Task SendSmsAsync(string otp, string mobileNumber)
    {
        try
        {
            string user = _config["Sms:User"];
            string authKey = _config["Sms:AuthKey"];
            string senderId = _config["Sms:SenderId"];
            DateTime now = DateTime.Now;
            string dateTime = now.ToString("dd-MM-yyyy HH:mm:ss");

            //string message =
            //    $"Dear Customer, Your OTP for UPI transaction is {otp}. Valid for 3 minutes. Do not share.";

            //string url =
            //    $"https://sms.soft-techsolutions.com/v3/sms/submit?" +
            //    $"user={user}&authkey={authKey}&senderid={senderId}" +
            //    $"&smstype=T&mobile={mobileNumber}&message={Uri.EscapeDataString(message)}";

            string message =
                $"Dear Customer, Your OTP for UPI transaction with THANE DCCB is: {otp} " +
                $"and is valid upto 3 mins: {dateTime}. Please do not share it with anyone";

            string url =
                $"https://sms.soft-techsolutions.com/v3/sms/submit?" +
                $"user={user}&authkey={authKey}&senderid={senderId}" +
                $"&smstype=T&mobile={mobileNumber}&message={Uri.EscapeDataString(message)}";


            await _http.GetAsync(url);
        }
        catch (Exception)
        {
            // log if needed
        }
    }
}
