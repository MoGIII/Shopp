using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Shopp.Services.CouponAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedCouponsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "CouponId", "CouponCode", "DiscountAmount", "ExpirationDate", "MinAmount" },
                values: new object[,]
                {
                    { 1, "10OFF", 10.0, new DateTime(2099, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 20 },
                    { 2, "20OFF", 20.0, new DateTime(2088, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 50 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "CouponId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "CouponId",
                keyValue: 2);
        }
    }
}
