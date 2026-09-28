namespace ChauChau.Models
{
    public class DonHang
    {
        public int DonHangId { get; set; }
        public int UserId { get; set; }

        public DateTime NgayDat { get; set; } = DateTime.Now;
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; } = "DangCho";

        // 🔥 Địa chỉ snapshot (lưu cố định để user đổi sau cũng không ảnh hưởng)
        public string HoTen { get; set; } = null!;
        public string SoDienThoai { get; set; } = null!;
        public string DiaChiChiTiet { get; set; } = null!;
        public string PhuongXa { get; set; } = null!;
        public string QuanHuyen { get; set; } = null!;
        public string TinhThanh { get; set; } = null!;

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public User User { get; set; } = null!;
        public ICollection<ChiTietDonHang>? ChiTietDonHangs { get; set; }
    }
}
