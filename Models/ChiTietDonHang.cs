namespace ChauChau.Models
{
    public class ChiTietDonHang
    {
        public int ChiTietDonHangId { get; set; }
        public int DonHangId { get; set; }
        public int SanPhamVariantId { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }

        public DonHang DonHang { get; set; } = null!;
        public SanPhamVariant SanPhamVariant { get; set; } = null!;
    }
}
    