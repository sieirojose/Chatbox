using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JoseSieiro_Chatbox_CalES.Migrations
{
    /// <inheritdoc />
    public partial class addingAtachedDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachedDocument",
                table: "Comments",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachedDocument",
                table: "Comments");
        }
    }
}
