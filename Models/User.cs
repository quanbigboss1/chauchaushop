namespace ChauChau.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string MatKhau { get; set; }
        public string SoDienThoai { get; set; }
        public string? Role { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public ICollection<DonHang>? DonHangs { get; set; }
        public ICollection<GioHang>? GioHangs { get; set; }
        public ICollection<DanhGia>? DanhGias { get; set; }
    }
}
