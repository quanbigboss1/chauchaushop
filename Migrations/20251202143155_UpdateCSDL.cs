using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChauChau.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCSDL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DanhMuc",
                columns: table => new
                {
                    DanhMucId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDanhMuc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMuc", x => x.DanhMucId);
                });

            migrationBuilder.CreateTable(
                name: "DonHang",
                columns: table => new
                {
                    DonHangId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    NgayDat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonHang", x => x.DonHangId);
                    table.ForeignKey(
                        name: "FK_DonHang_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SanPham",
                columns: table => new
                {
                    SanPhamId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenSanPham = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DanhMucId = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPham", x => x.SanPhamId);
                    table.ForeignKey(
                        name: "FK_SanPham_DanhMuc_DanhMucId",
                        column: x => x.DanhMucId,
                        principalTable: "DanhMuc",
                        principalColumn: "DanhMucId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SanPhamVariant",
                columns: table => new
                {
                    SanPhamVariantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SanPhamId = table.Column<int>(type: "int", nullable: false),
                    MauSac = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KichCo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPhamVariant", x => x.SanPhamVariantId);
                    table.ForeignKey(
                        name: "FK_SanPhamVariant_SanPham_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPham",
                        principalColumn: "SanPhamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonHang",
                columns: table => new
                {
                    ChiTietDonHangId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DonHangId = table.Column<int>(type: "int", nullable: false),
                    SanPhamVariantId = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDonHang", x => x.ChiTietDonHangId);
                    table.ForeignKey(
                        name: "FK_ChiTietDonHang_DonHang_DonHangId",
                        column: x => x.DonHangId,
                        principalTable: "DonHang",
                        principalColumn: "DonHangId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDonHang_SanPhamVariant_SanPhamVariantId",
                        column: x => x.SanPhamVariantId,
                        principalTable: "SanPhamVariant",
                        principalColumn: "SanPhamVariantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanhGia",
                columns: table => new
                {
                    DanhGiaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SanPhamVariantId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Diem = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhGia", x => x.DanhGiaId);
                    table.ForeignKey(
                        name: "FK_DanhGia_SanPhamVariant_SanPhamVariantId",
                        column: x => x.SanPhamVariantId,
                        principalTable: "SanPhamVariant",
                        principalColumn: "SanPhamVariantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DanhGia_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GioHang",
                columns: table => new
                {
                    GioHangId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SanPhamVariantId = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GioHang", x => x.GioHangId);
                    table.ForeignKey(
                        name: "FK_GioHang_SanPhamVariant_SanPhamVariantId",
                        column: x => x.SanPhamVariantId,
                        principalTable: "SanPhamVariant",
                        principalColumn: "SanPhamVariantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GioHang_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HinhAnhSanPhamVariant",
                columns: table => new
                {
                    HinhAnhId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SanPhamVariantId = table.Column<int>(type: "int", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HinhAnhSanPhamVariant", x => x.HinhAnhId);
                    table.ForeignKey(
                        name: "FK_HinhAnhSanPhamVariant_SanPhamVariant_SanPhamVariantId",
                        column: x => x.SanPhamVariantId,
                        principalTable: "SanPhamVariant",
                        principalColumn: "SanPhamVariantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_DonHangId",
                table: "ChiTietDonHang",
                column: "DonHangId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_SanPhamVariantId",
                table: "ChiTietDonHang",
                column: "SanPhamVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_SanPhamVariantId",
                table: "DanhGia",
                column: "SanPhamVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_UserId",
                table: "DanhGia",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_UserId",
                table: "DonHang",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GioHang_SanPhamVariantId",
                table: "GioHang",
                column: "SanPhamVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_GioHang_UserId",
                table: "GioHang",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HinhAnhSanPhamVariant_SanPhamVariantId",
                table: "HinhAnhSanPhamVariant",
                column: "SanPhamVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_DanhMucId",
                table: "SanPham",
                column: "DanhMucId");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamVariant_SanPhamId",
                table: "SanPhamVariant",
                column: "SanPhamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietDonHang");

            migrationBuilder.DropTable(
                name: "DanhGia");

            migrationBuilder.DropTable(
                name: "GioHang");

            migrationBuilder.DropTable(
                name: "HinhAnhSanPhamVariant");

            migrationBuilder.DropTable(
                name: "DonHang");

            migrationBuilder.DropTable(
                name: "SanPhamVariant");

            migrationBuilder.DropTable(
                name: "SanPham");

            migrationBuilder.DropTable(
                name: "DanhMuc");
        }
    }
}
