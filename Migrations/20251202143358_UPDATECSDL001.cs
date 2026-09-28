using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChauChau.Migrations
{
    /// <inheritdoc />
    public partial class UPDATECSDL001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHang_DonHang_DonHangId",
                table: "ChiTietDonHang");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHang_SanPhamVariant_SanPhamVariantId",
                table: "ChiTietDonHang");

            migrationBuilder.DropForeignKey(
                name: "FK_DanhGia_SanPhamVariant_SanPhamVariantId",
                table: "DanhGia");

            migrationBuilder.DropForeignKey(
                name: "FK_DanhGia_Users_UserId",
                table: "DanhGia");

            migrationBuilder.DropForeignKey(
                name: "FK_DonHang_Users_UserId",
                table: "DonHang");

            migrationBuilder.DropForeignKey(
                name: "FK_GioHang_SanPhamVariant_SanPhamVariantId",
                table: "GioHang");

            migrationBuilder.DropForeignKey(
                name: "FK_GioHang_Users_UserId",
                table: "GioHang");

            migrationBuilder.DropForeignKey(
                name: "FK_HinhAnhSanPhamVariant_SanPhamVariant_SanPhamVariantId",
                table: "HinhAnhSanPhamVariant");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPham_DanhMuc_DanhMucId",
                table: "SanPham");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhamVariant_SanPham_SanPhamId",
                table: "SanPhamVariant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SanPhamVariant",
                table: "SanPhamVariant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SanPham",
                table: "SanPham");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HinhAnhSanPhamVariant",
                table: "HinhAnhSanPhamVariant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GioHang",
                table: "GioHang");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DonHang",
                table: "DonHang");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DanhMuc",
                table: "DanhMuc");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DanhGia",
                table: "DanhGia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChiTietDonHang",
                table: "ChiTietDonHang");

            migrationBuilder.RenameTable(
                name: "SanPhamVariant",
                newName: "SanPhamVariants");

            migrationBuilder.RenameTable(
                name: "SanPham",
                newName: "SanPhams");

            migrationBuilder.RenameTable(
                name: "HinhAnhSanPhamVariant",
                newName: "HinhAnhSanPhamVariants");

            migrationBuilder.RenameTable(
                name: "GioHang",
                newName: "GioHangs");

            migrationBuilder.RenameTable(
                name: "DonHang",
                newName: "DonHangs");

            migrationBuilder.RenameTable(
                name: "DanhMuc",
                newName: "DanhMucs");

            migrationBuilder.RenameTable(
                name: "DanhGia",
                newName: "DanhGias");

            migrationBuilder.RenameTable(
                name: "ChiTietDonHang",
                newName: "ChiTietDonHangs");

            migrationBuilder.RenameIndex(
                name: "IX_SanPhamVariant_SanPhamId",
                table: "SanPhamVariants",
                newName: "IX_SanPhamVariants_SanPhamId");

            migrationBuilder.RenameIndex(
                name: "IX_SanPham_DanhMucId",
                table: "SanPhams",
                newName: "IX_SanPhams_DanhMucId");

            migrationBuilder.RenameIndex(
                name: "IX_HinhAnhSanPhamVariant_SanPhamVariantId",
                table: "HinhAnhSanPhamVariants",
                newName: "IX_HinhAnhSanPhamVariants_SanPhamVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_GioHang_UserId",
                table: "GioHangs",
                newName: "IX_GioHangs_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_GioHang_SanPhamVariantId",
                table: "GioHangs",
                newName: "IX_GioHangs_SanPhamVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_DonHang_UserId",
                table: "DonHangs",
                newName: "IX_DonHangs_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_DanhGia_UserId",
                table: "DanhGias",
                newName: "IX_DanhGias_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_DanhGia_SanPhamVariantId",
                table: "DanhGias",
                newName: "IX_DanhGias_SanPhamVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_ChiTietDonHang_SanPhamVariantId",
                table: "ChiTietDonHangs",
                newName: "IX_ChiTietDonHangs_SanPhamVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_ChiTietDonHang_DonHangId",
                table: "ChiTietDonHangs",
                newName: "IX_ChiTietDonHangs_DonHangId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SanPhamVariants",
                table: "SanPhamVariants",
                column: "SanPhamVariantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SanPhams",
                table: "SanPhams",
                column: "SanPhamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HinhAnhSanPhamVariants",
                table: "HinhAnhSanPhamVariants",
                column: "HinhAnhId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GioHangs",
                table: "GioHangs",
                column: "GioHangId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DonHangs",
                table: "DonHangs",
                column: "DonHangId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DanhMucs",
                table: "DanhMucs",
                column: "DanhMucId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DanhGias",
                table: "DanhGias",
                column: "DanhGiaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChiTietDonHangs",
                table: "ChiTietDonHangs",
                column: "ChiTietDonHangId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHangs_DonHangs_DonHangId",
                table: "ChiTietDonHangs",
                column: "DonHangId",
                principalTable: "DonHangs",
                principalColumn: "DonHangId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHangs_SanPhamVariants_SanPhamVariantId",
                table: "ChiTietDonHangs",
                column: "SanPhamVariantId",
                principalTable: "SanPhamVariants",
                principalColumn: "SanPhamVariantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGias_SanPhamVariants_SanPhamVariantId",
                table: "DanhGias",
                column: "SanPhamVariantId",
                principalTable: "SanPhamVariants",
                principalColumn: "SanPhamVariantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGias_Users_UserId",
                table: "DanhGias",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DonHangs_Users_UserId",
                table: "DonHangs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GioHangs_SanPhamVariants_SanPhamVariantId",
                table: "GioHangs",
                column: "SanPhamVariantId",
                principalTable: "SanPhamVariants",
                principalColumn: "SanPhamVariantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GioHangs_Users_UserId",
                table: "GioHangs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HinhAnhSanPhamVariants_SanPhamVariants_SanPhamVariantId",
                table: "HinhAnhSanPhamVariants",
                column: "SanPhamVariantId",
                principalTable: "SanPhamVariants",
                principalColumn: "SanPhamVariantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhams_DanhMucs_DanhMucId",
                table: "SanPhams",
                column: "DanhMucId",
                principalTable: "DanhMucs",
                principalColumn: "DanhMucId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhamVariants_SanPhams_SanPhamId",
                table: "SanPhamVariants",
                column: "SanPhamId",
                principalTable: "SanPhams",
                principalColumn: "SanPhamId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHangs_DonHangs_DonHangId",
                table: "ChiTietDonHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHangs_SanPhamVariants_SanPhamVariantId",
                table: "ChiTietDonHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_DanhGias_SanPhamVariants_SanPhamVariantId",
                table: "DanhGias");

            migrationBuilder.DropForeignKey(
                name: "FK_DanhGias_Users_UserId",
                table: "DanhGias");

            migrationBuilder.DropForeignKey(
                name: "FK_DonHangs_Users_UserId",
                table: "DonHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_GioHangs_SanPhamVariants_SanPhamVariantId",
                table: "GioHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_GioHangs_Users_UserId",
                table: "GioHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_HinhAnhSanPhamVariants_SanPhamVariants_SanPhamVariantId",
                table: "HinhAnhSanPhamVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhams_DanhMucs_DanhMucId",
                table: "SanPhams");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhamVariants_SanPhams_SanPhamId",
                table: "SanPhamVariants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SanPhamVariants",
                table: "SanPhamVariants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SanPhams",
                table: "SanPhams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HinhAnhSanPhamVariants",
                table: "HinhAnhSanPhamVariants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GioHangs",
                table: "GioHangs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DonHangs",
                table: "DonHangs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DanhMucs",
                table: "DanhMucs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DanhGias",
                table: "DanhGias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChiTietDonHangs",
                table: "ChiTietDonHangs");

            migrationBuilder.RenameTable(
                name: "SanPhamVariants",
                newName: "SanPhamVariant");

            migrationBuilder.RenameTable(
                name: "SanPhams",
                newName: "SanPham");

            migrationBuilder.RenameTable(
                name: "HinhAnhSanPhamVariants",
                newName: "HinhAnhSanPhamVariant");

            migrationBuilder.RenameTable(
                name: "GioHangs",
                newName: "GioHang");

            migrationBuilder.RenameTable(
                name: "DonHangs",
                newName: "DonHang");

            migrationBuilder.RenameTable(
                name: "DanhMucs",
                newName: "DanhMuc");

            migrationBuilder.RenameTable(
                name: "DanhGias",
                newName: "DanhGia");

            migrationBuilder.RenameTable(
                name: "ChiTietDonHangs",
                newName: "ChiTietDonHang");

            migrationBuilder.RenameIndex(
                name: "IX_SanPhamVariants_SanPhamId",
                table: "SanPhamVariant",
                newName: "IX_SanPhamVariant_SanPhamId");

            migrationBuilder.RenameIndex(
                name: "IX_SanPhams_DanhMucId",
                table: "SanPham",
                newName: "IX_SanPham_DanhMucId");

            migrationBuilder.RenameIndex(
                name: "IX_HinhAnhSanPhamVariants_SanPhamVariantId",
                table: "HinhAnhSanPhamVariant",
                newName: "IX_HinhAnhSanPhamVariant_SanPhamVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_GioHangs_UserId",
                table: "GioHang",
                newName: "IX_GioHang_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_GioHangs_SanPhamVariantId",
                table: "GioHang",
                newName: "IX_GioHang_SanPhamVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_DonHangs_UserId",
                table: "DonHang",
                newName: "IX_DonHang_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_DanhGias_UserId",
                table: "DanhGia",
                newName: "IX_DanhGia_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_DanhGias_SanPhamVariantId",
                table: "DanhGia",
                newName: "IX_DanhGia_SanPhamVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_ChiTietDonHangs_SanPhamVariantId",
                table: "ChiTietDonHang",
                newName: "IX_ChiTietDonHang_SanPhamVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_ChiTietDonHangs_DonHangId",
                table: "ChiTietDonHang",
                newName: "IX_ChiTietDonHang_DonHangId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SanPhamVariant",
                table: "SanPhamVariant",
                column: "SanPhamVariantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SanPham",
                table: "SanPham",
                column: "SanPhamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HinhAnhSanPhamVariant",
                table: "HinhAnhSanPhamVariant",
                column: "HinhAnhId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GioHang",
                table: "GioHang",
                column: "GioHangId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DonHang",
                table: "DonHang",
                column: "DonHangId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DanhMuc",
                table: "DanhMuc",
                column: "DanhMucId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DanhGia",
                table: "DanhGia",
                column: "DanhGiaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChiTietDonHang",
                table: "ChiTietDonHang",
                column: "ChiTietDonHangId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHang_DonHang_DonHangId",
                table: "ChiTietDonHang",
                column: "DonHangId",
                principalTable: "DonHang",
                principalColumn: "DonHangId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHang_SanPhamVariant_SanPhamVariantId",
                table: "ChiTietDonHang",
                column: "SanPhamVariantId",
                principalTable: "SanPhamVariant",
                principalColumn: "SanPhamVariantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGia_SanPhamVariant_SanPhamVariantId",
                table: "DanhGia",
                column: "SanPhamVariantId",
                principalTable: "SanPhamVariant",
                principalColumn: "SanPhamVariantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGia_Users_UserId",
                table: "DanhGia",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DonHang_Users_UserId",
                table: "DonHang",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GioHang_SanPhamVariant_SanPhamVariantId",
                table: "GioHang",
                column: "SanPhamVariantId",
                principalTable: "SanPhamVariant",
                principalColumn: "SanPhamVariantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GioHang_Users_UserId",
                table: "GioHang",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HinhAnhSanPhamVariant_SanPhamVariant_SanPhamVariantId",
                table: "HinhAnhSanPhamVariant",
                column: "SanPhamVariantId",
                principalTable: "SanPhamVariant",
                principalColumn: "SanPhamVariantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SanPham_DanhMuc_DanhMucId",
                table: "SanPham",
                column: "DanhMucId",
                principalTable: "DanhMuc",
                principalColumn: "DanhMucId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhamVariant_SanPham_SanPhamId",
                table: "SanPhamVariant",
                column: "SanPhamId",
                principalTable: "SanPham",
                principalColumn: "SanPhamId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
