namespace ChauChau.Models
{
    public class SanPham
    {
        public int SanPhamId { get; set; }
        public string TenSanPham { get; set; } = null!;
        public string? MoTa { get; set; }
        public int DanhMucId { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DanhMuc DanhMuc { get; set; } = null!;
        public ICollection<SanPhamVariant>? Variants { get; set; }
    }
}
