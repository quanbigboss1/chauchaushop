using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class MomoService : IMomoService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public string PartnerCode { get; private set; }
    public string AccessKey { get; private set; }
    public string SecretKey { get; private set; }
    public string ReturnUrl { get; private set; }
    public string NotifyUrl { get; private set; }
    public string RequestType { get; private set; }
    public string Endpoint { get; private set; }

    public MomoService(IConfiguration config)
    {
        _config = config;
        _httpClient = new HttpClient();

        PartnerCode = _config["Momo:PartnerCode"];
        AccessKey = _config["Momo:AccessKey"];
        SecretKey = _config["Momo:SecretKey"];
        ReturnUrl = _config["Momo:ReturnUrl"];
        NotifyUrl = _config["Momo:NotifyUrl"];
        RequestType = _config["Momo:RequestType"];
        Endpoint = _config["Momo:Endpoint"];
    }

    public async Task<string> CreatePaymentUrl(HttpContext context, decimal amount, int orderId)
    {
        string orderInfo = $"Thanh toán đơn hàng #{orderId}";
        string requestId = Guid.NewGuid().ToString();
        string extraData = "";

        string amountStr = ((long)amount).ToString(); // Momo yêu cầu số nguyên
        string rawHash = $"accessKey={AccessKey}&amount={amountStr}&extraData={extraData}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={PartnerCode}&redirectUrl={ReturnUrl}&requestId={requestId}&requestType={RequestType}";
        string signature = HmacSHA256(rawHash, SecretKey);

        var requestBody = new
        {
            partnerCode = PartnerCode,
            accessKey = AccessKey,
            requestId = requestId,
            amount = amountStr,
            orderId = orderId.ToString(),
            orderInfo = orderInfo,
            returnUrl = ReturnUrl,
            notifyUrl = NotifyUrl,
            extraData = extraData,
            requestType = RequestType,
            signature = signature
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(Endpoint, content);
        var respString = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(respString);
        var payUrl = doc.RootElement.GetProperty("payUrl").GetString();

        return payUrl;
    }

    public bool ValidateSignature(string signature, string rawData, string secretKey)
    {
        string hash = HmacSHA256(rawData, secretKey);
        return hash.Equals(signature, StringComparison.OrdinalIgnoreCase);
    }

    private string HmacSHA256(string data, string key)
    {
        var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
}
