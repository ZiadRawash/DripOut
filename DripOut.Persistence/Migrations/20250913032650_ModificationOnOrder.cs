using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DripOut.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModificationOnOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShippingGovernorateNameSnapshot",
                table: "Orders",
                newName: "GovernorateNameSnapshot");

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotal",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubTotal",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "GovernorateNameSnapshot",
                table: "Orders",
                newName: "ShippingGovernorateNameSnapshot");


        }
    }
}
