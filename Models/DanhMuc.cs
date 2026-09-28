namespace ChauChau.Models
{
    public class DanhMuc
    {
        public int DanhMucId { get; set; }
        public string TenDanhMuc { get; set; } = null!;
        public string? MoTa { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public ICollection<SanPham>? SanPhams { get; set; }
    }
}
