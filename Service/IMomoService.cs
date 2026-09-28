using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public interface IMomoService
{
    Task<string> CreatePaymentUrl(HttpContext context, decimal amount, int orderId);
    bool ValidateSignature(string signature, string rawData, string secretKey);

    // Cấu hình
    string PartnerCode { get; }
    string AccessKey { get; }
    string SecretKey { get; }
    string ReturnUrl { get; }
    string NotifyUrl { get; }
}
