using System.ComponentModel.DataAnnotations;

namespace ChauChau.Models
{
    public class HinhAnhSanPhamVariant
    {
        [Key]
        public int HinhAnhId { get; set; }  // ← Đây phải là PK

        public int SanPhamVariantId { get; set; }
        public string HinhAnh { get; set; } = null!;
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public SanPhamVariant SanPhamVariant { get; set; } = null!;
    }
}
