using Microsoft.EntityFrameworkCore.Migrations;

namespace BookStore.Migrations
{
    public partial class BookStore_Orders_201018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Book_BookFK",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_BookFK",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "BookFK",
                table: "Book",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Book_BookFK",
                table: "Book",
                column: "BookFK");

            migrationBuilder.AddForeignKey(
                name: "FK_Book_Orders_BookFK",
                table: "Book",
                column: "BookFK",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_Orders_BookFK",
                table: "Book");

            migrationBuilder.DropIndex(
                name: "IX_Book_BookFK",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "BookFK",
                table: "Book");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BookFK",
                table: "Orders",
                column: "BookFK");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Book_BookFK",
                table: "Orders",
                column: "BookFK",
                principalTable: "Book",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
