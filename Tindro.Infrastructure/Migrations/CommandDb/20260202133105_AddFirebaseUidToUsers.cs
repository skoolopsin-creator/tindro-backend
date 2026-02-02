using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tindro.Infrastructure.Migrations.CommandDb
{
    /// <inheritdoc />
    public partial class AddFirebaseUidToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirebaseUid",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirebaseUid",
                table: "Users");
        }
    }
}
