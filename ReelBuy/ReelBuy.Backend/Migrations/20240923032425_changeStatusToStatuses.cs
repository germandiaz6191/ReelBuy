using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReelBuy.Backend.Migrations
{
    /// <inheritdoc />
    public partial class changeStatusToStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Status",
                table: "Status");

            migrationBuilder.RenameTable(
                name: "Status",
                newName: "Statuses");

            migrationBuilder.RenameIndex(
                name: "IX_Status_Name",
                table: "Statuses",
                newName: "IX_Statuses_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Statuses",
                table: "Statuses",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Statuses",
                table: "Statuses");

            migrationBuilder.RenameTable(
                name: "Statuses",
                newName: "Status");

            migrationBuilder.RenameIndex(
                name: "IX_Statuses_Name",
                table: "Status",
                newName: "IX_Status_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Status",
                table: "Status",
                column: "Id");
        }
    }
}
