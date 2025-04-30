using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReelBuy.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddLike : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Likes",
                table: "Products",
                newName: "LikesGroup");

            migrationBuilder.CreateTable(
                name: "UserProductLikes",
                columns: table => new
                {
                    LikedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LikesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProductLikes", x => new { x.LikedById, x.LikesId });
                    table.ForeignKey(
                        name: "FK_UserProductLikes_AspNetUsers_LikedById",
                        column: x => x.LikedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserProductLikes_Products_LikesId",
                        column: x => x.LikesId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProductLikes_LikesId",
                table: "UserProductLikes",
                column: "LikesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProductLikes");

            migrationBuilder.RenameColumn(
                name: "LikesGroup",
                table: "Products",
                newName: "Likes");
        }
    }
}
