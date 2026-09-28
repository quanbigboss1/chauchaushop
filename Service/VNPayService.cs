using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System; // Cần thêm System cho DateTime

namespace ChauChau.Services
{
    public interface IVNPayService
    {
        string CreatePaymentUrl(HttpContext context, decimal amount, int orderId);
        VNPayResponse ProcessReturn(IQueryCollection query);
    }

    public class VNPayService : IVNPayService
    {
        private readonly IConfiguration _config;

        public VNPayService(IConfiguration config)
        {
            _config = config;
        }

        public string CreatePaymentUrl(HttpContext context, decimal amount, int orderId)
        {
            string vnp_TmnCode = _config["VNPay:TmnCode"];
            string vnp_HashSecret = _config["VNPay:HashSecret"];
            string vnp_Url = _config["VNPay:BaseUrl"];
            string vnp_ReturnUrl = _config["VNPay:ReturnUrl"]; // URL controller VNPayReturn của bạn

            VnPayLibrary vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((long)(amount * 100)).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1");
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toán đơn hàng #{orderId}");
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", orderId.ToString());
            Console.WriteLine("===== VNPAY DEBUG =====");
            Console.WriteLine("TmnCode: " + vnp_TmnCode);
            Console.WriteLine("BaseUrl: " + vnp_Url);
            Console.WriteLine("ReturnUrl: " + vnp_ReturnUrl);
            Console.WriteLine("Amount: " + amount);
            Console.WriteLine("OrderId: " + orderId);
            Console.WriteLine("=======================");
            // Tạo URL VNPay hoàn chỉnh
            return vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
        }


        public VNPayResponse ProcessReturn(IQueryCollection query)
        {
            var vnp_HashSecret = _config["VNPay:HashSecret"];
            var vnpay = new VnPayLibrary();

            // Xử lý trường hợp không có vnp_SecureHash
            var secureHash = query["vnp_SecureHash"].ToString();

            bool isValid = vnpay.ValidateSignature(secureHash, vnp_HashSecret, query);

            // 💡 SỬA LỖI KIỂU DỮ LIỆU: Parse sang decimal và sử dụng TryParse an toàn hơn
            decimal amount = 0;
            if (long.TryParse(query["vnp_Amount"], out long rawAmount))
            {
                amount = (decimal)rawAmount / 100;
            }

            int orderId = 0;
            if (int.TryParse(query["vnp_TxnRef"], out int parsedOrderId))
            {
                orderId = parsedOrderId;
            }

            return new VNPayResponse
            {
                Success = isValid && query["vnp_ResponseCode"] == "00",
                Amount = amount, // ✅ Đã là decimal
                OrderId = orderId,
                Message = query["vnp_ResponseCode"]
            };
        }
    }

    public class VNPayResponse
    {
        public bool Success { get; set; }
        public int OrderId { get; set; }
        // 💡 SỬA LỖI KIỂU DỮ LIỆU: Đã đổi từ long sang decimal
        public decimal Amount { get; set; }
        public string Message { get; set; }
    }
}