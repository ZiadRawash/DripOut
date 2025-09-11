using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DripOut.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class update_order : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ShippingCostSnapShot",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ShippingGovernorateNameSnapshot",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Governorates",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingCostSnapShot",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingGovernorateNameSnapshot",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Governorates");
        }
    }
}
