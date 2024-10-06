using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Razor.Migrations
{
    /// <inheritdoc />
    public partial class RemovePasswordHashColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Authors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Authors",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
