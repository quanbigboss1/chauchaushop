using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChauChau.Migrations
{
    /// <inheritdoc />
    public partial class update12032139219312 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiaChiChiTiet",
                table: "DonHangs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HoTen",
                table: "DonHangs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhuongXa",
                table: "DonHangs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "QuanHuyen",
                table: "DonHangs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SoDienThoai",
                table: "DonHangs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TinhThanh",
                table: "DonHangs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiaChiChiTiet",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "HoTen",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "PhuongXa",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "QuanHuyen",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "SoDienThoai",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "TinhThanh",
                table: "DonHangs");
        }
    }
}
