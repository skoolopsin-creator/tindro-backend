using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tindro.Infrastructure.Migrations.CommandDb
{
    /// <inheritdoc />
    public partial class PostEnhancement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_interests_Category",
                table: "user_interests");

            migrationBuilder.DropIndex(
                name: "IX_user_interests_UserId_InterestName",
                table: "user_interests");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "user_interests");

            migrationBuilder.DropColumn(
                name: "InterestName",
                table: "user_interests");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Posts",
                newName: "Title");

            migrationBuilder.AddColumn<Guid>(
                name: "InterestId",
                table: "user_interests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Posts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ShareCount",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Posts",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "interests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IconKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_interests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_interests_InterestId",
                table: "user_interests",
                column: "InterestId");

            migrationBuilder.CreateIndex(
                name: "IX_user_interests_UserId_InterestId",
                table: "user_interests",
                columns: new[] { "UserId", "InterestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_interests_Name",
                table: "interests",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_user_interests_Users_UserId",
                table: "user_interests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_interests_interests_InterestId",
                table: "user_interests",
                column: "InterestId",
                principalTable: "interests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_interests_Users_UserId",
                table: "user_interests");

            migrationBuilder.DropForeignKey(
                name: "FK_user_interests_interests_InterestId",
                table: "user_interests");

            migrationBuilder.DropTable(
                name: "interests");

            migrationBuilder.DropIndex(
                name: "IX_user_interests_InterestId",
                table: "user_interests");

            migrationBuilder.DropIndex(
                name: "IX_user_interests_UserId_InterestId",
                table: "user_interests");

            migrationBuilder.DropColumn(
                name: "InterestId",
                table: "user_interests");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ShareCount",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Posts",
                newName: "Content");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "user_interests",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InterestName",
                table: "user_interests",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_user_interests_Category",
                table: "user_interests",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_user_interests_UserId_InterestName",
                table: "user_interests",
                columns: new[] { "UserId", "InterestName" },
                unique: true);
        }
    }
}
