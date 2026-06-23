using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LiveLifeCoffee.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserFullName = table.Column<string>(type: "TEXT", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "TEXT", nullable: false),
                    OrderedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    MenuItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "Category", "Description", "IsAvailable", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Sıcak İçecekler", "Taze çekilmiş, yumuşak ve aromatik filtre kahve.", true, "Filtre Kahve", 65.00m },
                    { 2, "Sıcak İçecekler", "Yoğun ve güçlü İtalyan usulü espresso.", true, "Espresso", 55.00m },
                    { 3, "Sıcak İçecekler", "Sıcak suyla seyreltilmiş, yumuşak içimli espresso.", true, "Americano", 70.00m },
                    { 4, "Sıcak İçecekler", "Bol sıcak süt ve hafif süt köpüğü ile espresso.", true, "Latte", 85.00m },
                    { 5, "Sıcak İçecekler", "Eşit oranda espresso, sıcak süt ve yoğun süt köpüğü.", true, "Cappuccino", 85.00m },
                    { 6, "Sıcak İçecekler", "Çikolata şurubu, espresso, sıcak süt ve krema.", true, "Mocha", 95.00m },
                    { 7, "Soğuk İçecekler", "Buz üzerine soğuk süt ve espresso.", true, "Iced Latte", 90.00m },
                    { 8, "Soğuk İçecekler", "Buz ve soğuk su ile hazırlanmış ferahlatıcı espresso.", true, "Iced Americano", 75.00m },
                    { 9, "Soğuk İçecekler", "12 saat soğuk suda demlenmiş pürüzsüz kahve.", true, "Cold Brew", 110.00m },
                    { 10, "Soğuk İçecekler", "Buzla karıştırılmış, kremalı soğuk kahve tatlısı.", true, "Frappuccino", 120.00m },
                    { 11, "Soğuk İçecekler", "Taze sıkılmış limon ve nane yaprakları ile.", true, "Limonata", 60.00m },
                    { 12, "Atıştırmalıklar", "İçi akışkan, üstü yanık enfes İspanyol keki.", true, "San Sebastian Cheesecake", 140.00m },
                    { 13, "Atıştırmalıklar", "Yoğun çikolatalı, fıstık parçalı ıslak kek.", true, "Brownie", 55.00m },
                    { 14, "Atıştırmalıklar", "Tam buğday ekmeği üzerine taze avokado, limon ve kırmızı biber.", true, "Avokado Toast", 90.00m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_OrderId",
                table: "CartItems",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "ContactMessages");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
