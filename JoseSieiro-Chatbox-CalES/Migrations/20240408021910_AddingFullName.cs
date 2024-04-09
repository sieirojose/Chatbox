using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JoseSieiro_Chatbox_CalES.Migrations
{
    /// <inheritdoc />
    public partial class AddingFullName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.AddColumn<string>(
	            name: "FullName",
	            table: "Users",
	            type: "TEXT",
	            nullable: false,
	            defaultValue: "");

		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Users");
        }
    }
}
