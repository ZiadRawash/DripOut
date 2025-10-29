using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DripOut.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReservationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orderItems_Orders_OrderId",
                table: "orderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_orderItems_Products_ProductId",
                table: "orderItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_orderItems",
                table: "orderItems");

            migrationBuilder.DropColumn(
                name: "ShippingCost",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingName",
                table: "Orders");

            migrationBuilder.RenameTable(
                name: "orderItems",
                newName: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "OrderItems",
                newName: "ProductVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_orderItems_OrderId",
                table: "OrderItems",
                newName: "IX_OrderItems_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_orderItems_ProductId",
                table: "OrderItems",
                newName: "IX_OrderItems_ProductVariantId");

            migrationBuilder.RenameColumn(
                name: "MinimumOrderForFreeShipping",
                table: "Governorates",
                newName: "Threshold");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountApplied",
                table: "OrderItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "StockReservation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductVariantId = table.Column<int>(type: "int", nullable: false),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderItemId1 = table.Column<int>(type: "int", nullable: true),
                    ProductVariantId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockReservation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockReservation_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockReservation_OrderItems_OrderItemId1",
                        column: x => x.OrderItemId1,
                        principalTable: "OrderItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockReservation_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockReservation_ProductVariants_ProductVariantId1",
                        column: x => x.ProductVariantId1,
                        principalTable: "ProductVariants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockReservation_OrderItemId",
                table: "StockReservation",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockReservation_OrderItemId1",
                table: "StockReservation",
                column: "OrderItemId1",
                unique: true,
                filter: "[OrderItemId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StockReservation_ProductVariantId",
                table: "StockReservation",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_StockReservation_ProductVariantId1",
                table: "StockReservation",
                column: "ProductVariantId1");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ProductVariants_ProductVariantId",
                table: "OrderItems",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ProductVariants_ProductVariantId",
                table: "OrderItems");

            migrationBuilder.DropTable(
                name: "StockReservation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "DiscountApplied",
                table: "OrderItems");

            migrationBuilder.RenameTable(
                name: "OrderItems",
                newName: "orderItems");

            migrationBuilder.RenameColumn(
                name: "ProductVariantId",
                table: "orderItems",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_OrderId",
                table: "orderItems",
                newName: "IX_orderItems_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_ProductVariantId",
                table: "orderItems",
                newName: "IX_orderItems_ProductId");

            migrationBuilder.RenameColumn(
                name: "Threshold",
                table: "Governorates",
                newName: "MinimumOrderForFreeShipping");

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingCost",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ShippingName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_orderItems",
                table: "orderItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_orderItems_Orders_OrderId",
                table: "orderItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_orderItems_Products_ProductId",
                table: "orderItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
