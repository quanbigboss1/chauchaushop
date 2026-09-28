namespace ChauChau.Helpers
{
    public class VNPayConfig
    {
        public static string vnp_TmnCode = "JM275CAK"; // Mã website tại VNPay
        public static string vnp_HashSecret = "KMOAVQTFIXKHRYQDLMFQZBFQUWQCPSKP"; // Secret key VNPay cung cấp
        public static string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"; // URL VNPay sandbox
        public static string vnp_ReturnUrl = "https://localhost:7097/Test/PaymentConfirm";

    }
}
