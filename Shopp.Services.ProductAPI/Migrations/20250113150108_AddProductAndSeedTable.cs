using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Shopp.Services.ProductAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddProductAndSeedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryName", "Description", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Mouse", "An ergonomic mouse for less joint pain", "https://placehold.co/603x403", "Ergonomic Mouse", 50.990000000000002 },
                    { 2, "Keyboard", "Top tier gaming keyboard", "https://placehold.co/602x402", "Gaming Keyboard", 133.99000000000001 },
                    { 3, "Monitor", "Gaming curved monitor X\" and refresh rate of YYY Mhz", "https://placehold.co/601x401", "Monitor", 299.99000000000001 },
                    { 4, "Headphones", "Basic office headphones", "https://placehold.co/600x400", "Office Headphones", 15.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
