using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReelBuy.Backend.Migrations
{
    /// <inheritdoc />
    public partial class reels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Reels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reels_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_StatusId",
                table: "Products",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Reels_Name",
                table: "Reels",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reels_ProductId",
                table: "Reels",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Statuses_StatusId",
                table: "Products",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Statuses_StatusId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Reels");

            migrationBuilder.DropIndex(
                name: "IX_Products_StatusId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Products");
        }
    }
}
