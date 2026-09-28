namespace ChauChau.Models
{
    public class SanPhamVariant
    {
        public int SanPhamVariantId { get; set; }
        public int SanPhamId { get; set; }
        public string MauSac { get; set; } = null!;
        public string KichCo { get; set; } = null!;
        public decimal Gia { get; set; }
        public int SoLuong { get; set; } = 0;

        public SanPham SanPham { get; set; } = null!;
        public ICollection<HinhAnhSanPhamVariant>? HinhAnhs { get; set; }
        public ICollection<ChiTietDonHang>? ChiTietDonHangs { get; set; }
        public ICollection<GioHang>? GioHangs { get; set; }
        public ICollection<DanhGia>? DanhGias { get; set; }
    }
}
