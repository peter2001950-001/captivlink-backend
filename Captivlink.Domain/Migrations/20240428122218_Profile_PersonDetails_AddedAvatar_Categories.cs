using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Captivlink.Infrastructure.Migrations
{
    public partial class Profile_PersonDetails_AddedAvatar_Categories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "PersonDetails",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "PersonDetails",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CategoryPersonDetails",
                columns: table => new
                {
                    CategoriesId = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonDetailsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryPersonDetails", x => new { x.CategoriesId, x.PersonDetailsId });
                    table.ForeignKey(
                        name: "FK_CategoryPersonDetails_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryPersonDetails_PersonDetails_PersonDetailsId",
                        column: x => x.PersonDetailsId,
                        principalTable: "PersonDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryPersonDetails_PersonDetailsId",
                table: "CategoryPersonDetails",
                column: "PersonDetailsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryPersonDetails");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "PersonDetails");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "PersonDetails");
        }
    }
}
