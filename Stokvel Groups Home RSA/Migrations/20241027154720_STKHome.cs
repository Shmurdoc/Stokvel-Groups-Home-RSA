using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stokvel_Groups_Home_RSA.Migrations
{
    public partial class STKHome : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Invoices_InvoiceId",
                table: "Deposits");

            migrationBuilder.DropIndex(
                name: "IX_Deposits_InvoiceId",
                table: "Deposits");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "Deposits");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InvoiceId",
                table: "Deposits",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deposits_InvoiceId",
                table: "Deposits",
                column: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Invoices_InvoiceId",
                table: "Deposits",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "InvoiceId");
        }
    }
}
