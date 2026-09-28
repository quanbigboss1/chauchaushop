using ChauChau.Models;
using Microsoft.EntityFrameworkCore;

namespace ChauChau.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<DanhMuc> DanhMucs { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<SanPhamVariant> SanPhamVariants { get; set; }
        public DbSet<HinhAnhSanPhamVariant> HinhAnhSanPhamVariants { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<DanhGia> DanhGias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ------------------ Decimal Precision ------------------
            modelBuilder.Entity<SanPhamVariant>()
                        .Property(v => v.Gia)
                        .HasPrecision(18, 2);

            modelBuilder.Entity<ChiTietDonHang>()
                        .Property(c => c.Gia)
                        .HasPrecision(18, 2);

            modelBuilder.Entity<DonHang>()
                        .Property(d => d.TongTien)
                        .HasPrecision(18, 2);

            // ------------------ Thiết lập quan hệ 1-n ------------------
            modelBuilder.Entity<SanPham>()
                        .HasMany(s => s.Variants)
                        .WithOne(v => v.SanPham)
                        .HasForeignKey(v => v.SanPhamId);

            modelBuilder.Entity<SanPhamVariant>()
                        .HasMany(v => v.HinhAnhs)
                        .WithOne(h => h.SanPhamVariant)
                        .HasForeignKey(h => h.SanPhamVariantId);

            modelBuilder.Entity<User>()
                        .HasMany(u => u.DonHangs)
                        .WithOne(d => d.User)
                        .HasForeignKey(d => d.UserId);

            modelBuilder.Entity<DonHang>()
                        .HasMany(d => d.ChiTietDonHangs)
                        .WithOne(c => c.DonHang)
                        .HasForeignKey(c => c.DonHangId);

            modelBuilder.Entity<SanPhamVariant>()
                        .HasMany(v => v.ChiTietDonHangs)
                        .WithOne(c => c.SanPhamVariant)
                        .HasForeignKey(c => c.SanPhamVariantId);

            modelBuilder.Entity<User>()
                        .HasMany(u => u.GioHangs)
                        .WithOne(g => g.User)
                        .HasForeignKey(g => g.UserId);

            modelBuilder.Entity<SanPhamVariant>()
                        .HasMany(v => v.GioHangs)
                        .WithOne(g => g.SanPhamVariant)
                        .HasForeignKey(g => g.SanPhamVariantId);

            modelBuilder.Entity<SanPhamVariant>()
                        .HasMany(v => v.DanhGias)
                        .WithOne(dg => dg.SanPhamVariant)
                        .HasForeignKey(dg => dg.SanPhamVariantId);

            modelBuilder.Entity<User>()
                        .HasMany(u => u.DanhGias)
                        .WithOne(dg => dg.User)
                        .HasForeignKey(dg => dg.UserId);

            // ------------------ Đảm bảo Primary Key cho các entity ------------------
            modelBuilder.Entity<HinhAnhSanPhamVariant>()
                        .HasKey(h => h.HinhAnhId);
            modelBuilder.Entity<SanPhamVariant>()
                        .HasKey(v => v.SanPhamVariantId);
            modelBuilder.Entity<SanPham>()
                        .HasKey(s => s.SanPhamId);
            modelBuilder.Entity<DanhMuc>()
                        .HasKey(d => d.DanhMucId);
            modelBuilder.Entity<User>()
                        .HasKey(u => u.UserId);
            modelBuilder.Entity<DonHang>()
                        .HasKey(d => d.DonHangId);
            modelBuilder.Entity<ChiTietDonHang>()
                        .HasKey(c => c.ChiTietDonHangId);
            modelBuilder.Entity<GioHang>()
                        .HasKey(g => g.GioHangId);
            modelBuilder.Entity<DanhGia>()
                        .HasKey(dg => dg.DanhGiaId);
        }
    }
}
