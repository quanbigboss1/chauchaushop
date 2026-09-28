using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

public class VnPayLibrary
{
    private readonly SortedList<string, string> requestData =
        new SortedList<string, string>();

    public void AddRequestData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            requestData.Add(key, value);
        }
    }

    public string CreateRequestUrl(string baseUrl, string secretKey)
    {
        var hashData = new StringBuilder();
        var query = new StringBuilder();

        foreach (var kv in requestData)
        {
            if (string.IsNullOrEmpty(kv.Value))
                continue;

            // ==============================
            // DATA DÙNG ĐỂ HASH
            // VNPay yêu cầu hash giá trị gốc
            // ==============================
            if (hashData.Length > 0)
                hashData.Append("&");

            hashData.Append(kv.Key);
            hashData.Append("=");
            hashData.Append(kv.Value);

            // ==============================
            // DATA DÙNG ĐỂ TẠO QUERY URL
            // ==============================
            if (query.Length > 0)
                query.Append("&");

            query.Append(
                Uri.EscapeDataString(kv.Key)
            );

            query.Append("=");

            query.Append(
                Uri.EscapeDataString(kv.Value)
            );
        }

        // Tạo HMAC SHA512 từ hashData
        string secureHash =
            HmacSHA512(
                secretKey,
                hashData.ToString()
            );

        query.Append("&vnp_SecureHash=");
        query.Append(secureHash);

        return baseUrl + "?" + query;
    }

    public bool ValidateSignature(
        string vnpSecureHash,
        string secretKey,
        IQueryCollection query)
    {
        if (string.IsNullOrEmpty(vnpSecureHash))
            return false;

        var data = new SortedDictionary<string, string>(
            StringComparer.Ordinal
        );

        foreach (var item in query)
        {
            string key = item.Key;

            // Chỉ lấy tham số VNPay
            if (!key.StartsWith("vnp_"))
                continue;

            // Không đưa chữ ký vào dữ liệu hash
            if (key.Equals(
                "vnp_SecureHash",
                StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // Không đưa SecureHashType vào hash
            if (key.Equals(
                "vnp_SecureHashType",
                StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string value = item.Value.ToString();

            if (string.IsNullOrEmpty(value))
                continue;

            data[key] = value;
        }

        // ========================================
        // TẠO HASH DATA
        // ========================================
        var hashData = new StringBuilder();

        foreach (var item in data)
        {
            if (hashData.Length > 0)
                hashData.Append("&");

            hashData.Append(item.Key);
            hashData.Append("=");
            hashData.Append(item.Value);
        }

        string computedHash =
            HmacSHA512(
                secretKey,
                hashData.ToString()
            );

        return computedHash.Equals(
            vnpSecureHash,
            StringComparison.OrdinalIgnoreCase
        );
    }

    private string HmacSHA512(
        string key,
        string input)
    {
        byte[] keyBytes =
            Encoding.UTF8.GetBytes(key);

        byte[] inputBytes =
            Encoding.UTF8.GetBytes(input);

        using var hmac =
            new HMACSHA512(keyBytes);

        byte[] hash =
            hmac.ComputeHash(inputBytes);

        return Convert.ToHexString(hash)
            .ToLowerInvariant();
    }
}