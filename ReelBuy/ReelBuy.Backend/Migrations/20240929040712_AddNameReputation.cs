using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReelBuy.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddNameReputation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Reputations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Reputations");
        }
    }
}
