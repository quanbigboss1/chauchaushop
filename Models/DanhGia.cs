namespace ChauChau.Models
{
    public class DanhGia
    {
        public int DanhGiaId { get; set; }
        public int SanPhamVariantId { get; set; }
        public int UserId { get; set; }
        public int Diem { get; set; } // 1–5
        public string? NoiDung { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public SanPhamVariant SanPhamVariant { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
