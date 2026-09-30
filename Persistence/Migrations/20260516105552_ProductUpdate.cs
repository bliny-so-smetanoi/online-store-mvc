using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProductUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description_Code",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Description_En",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Description_Kk",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Name_Code",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Name_Kk",
                table: "Products",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Name_En",
                table: "Products",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Products",
                newName: "Name_Kk");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Products",
                newName: "Name_En");

            migrationBuilder.AddColumn<string>(
                name: "Description_Code",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description_En",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description_Kk",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name_Code",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
