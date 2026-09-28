namespace ChauChau.Models
{
    public class GioHang
    {
        public int GioHangId { get; set; }
        public int UserId { get; set; }
        public int SanPhamVariantId { get; set; }
        public int SoLuong { get; set; } = 1;
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public User User { get; set; } = null!;
        public SanPhamVariant SanPhamVariant { get; set; } = null!;
    }
}
