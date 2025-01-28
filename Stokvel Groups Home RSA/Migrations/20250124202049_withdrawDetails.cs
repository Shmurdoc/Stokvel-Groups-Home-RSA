using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stokvel_Groups_Home_RSA.Migrations
{
    public partial class withdrawDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccoutNumber",
                table: "WithdrawDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "WithdrawDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionReference",
                table: "WithdrawDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccoutNumber",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccoutNumber",
                table: "WithdrawDetails");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "WithdrawDetails");

            migrationBuilder.DropColumn(
                name: "TransactionReference",
                table: "WithdrawDetails");

            migrationBuilder.DropColumn(
                name: "AccoutNumber",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Accounts");
        }
    }
}
