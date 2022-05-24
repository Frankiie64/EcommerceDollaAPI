using Microsoft.EntityFrameworkCore.Migrations;

namespace ECommerce.Migrations
{
    public partial class REMOTE_DATABASE : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdStatus",
                table: "mercancias",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_pedido_IdStatus",
                table: "mercancias",
                column: "IdStatus");

            migrationBuilder.AddForeignKey(
                name: "FK_pedido_status_IdStatus",
                table: "mercancias",
                column: "IdStatus",
                principalTable: "status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pedido_status_IdStatus",
                table: "pedido");

            migrationBuilder.DropIndex(
                name: "IX_pedido_IdStatus",
                table: "pedido");

            migrationBuilder.DropColumn(
                name: "IdStatus",
                table: "pedido");
        }
    }
}
