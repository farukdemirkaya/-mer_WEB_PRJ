// =========================================================
// Data/AppDbContext.cs
// Entity Framework Core Veritabanı Bağlamı (Context)
// Veritabanı tablolarını ve başlangıç verilerini yapılandırır.
// =========================================================

using LiveLifeCoffee.Models;
using Microsoft.EntityFrameworkCore;

namespace LiveLifeCoffee.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Veritabanındaki tablolarımız
        public DbSet<User> Users { get; set; }                          // Kullanıcı tablosu
        public DbSet<MenuItem> MenuItems { get; set; }                  // Menü ürünleri tablosu
        public DbSet<Order> Orders { get; set; }                        // Siparişler tablosu
        public DbSet<CartItem> CartItems { get; set; }                  // Sepet kalemleri tablosu
        public DbSet<ContactMessage> ContactMessages { get; set; }      // İletişim mesajları tablosu
        public DbSet<JobApplication> JobApplications { get; set; }      // İş başvuruları tablosu (YENİ)

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // İlişkiler: Bir siparişin (Order) birden çok kalemi (CartItem) vardır
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey(c => c.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Sipariş silinirse kalemleri de silinsin

            // Başlangıç verilerini (Menü öğeleri) SeedData olarak veritabanına ekliyoruz
            modelBuilder.Entity<MenuItem>().HasData(
                new MenuItem
                {
                    Id = 1,
                    Name = "Filtre Kahve",
                    Description = "Taze çekilmiş, yumuşak ve aromatik filtre kahve.",
                    Price = 65.00m,
                    Category = "Sıcak İçecekler",
                    IsAvailable = true
                },
                new MenuItem
                {
                    Id = 2,
                    Name = "Espresso",
                    Description = "Yoğun ve güçlü İtalyan usulü espresso.",
                    Price = 55.00m,
                    Category = "Sıcak İçecekler",
                    IsAvailable = true
                },
                new MenuItem
                {
                    Id = 3,
                    Name = "Americano",
                    Description = "Sıcak suyla seyreltilmiş, yumuşak içimli espresso.",
                    Price = 70.00m,
                    Category = "Sıcak İçecekler",
                    IsAvailable = true
                },
                new MenuItem
                {
                    Id = 4,
                    Name = "Latte",
                    Description = "Bol sıcak süt ve hafif süt köpüğü ile espresso.",
                    Price = 85.00m,
                    Category = "Sıcak İçecekler",
                    IsAvailable = true
                },
                new MenuItem
                {
                    Id = 5,
                    Name = "Cappuccino",
                    Description = "Eşit oranda espresso, sıcak süt ve yoğun süt köpüğü.",
                    Price = 85.00m,
                    Category = "Sıcak İçecekler",
                    IsAvailable = true
                },
                new MenuItem
                {
                    Id = 6,
                    Name = "Mocha",
                    Description = "Çikolata şurubu, espresso, sıcak süt ve krema.",
                    Price = 95.00m,
                    Category = "Sıcak İçecekler",
                    IsAvailable = true
                },
                new MenuItem
                {
                    Id = 7,
                    Name = "Iced Latte",
                    Description = "Buz üzerine soğuk süt ve espresso.",
                    Price = 90.00m,
                    Category = "Soğuk İçecekler",
                    IsAvailable = true
                },
                new MenuItem
                {
                    Id = 8,
                    Name = "Iced Americano",
                    Description = "Buz ve soğuk su ile hazırlanmış ferahlatıcı espresso.",
                    Price = 75.00m,
                    Category = "Soğuk İçecekler",
                    IsAvailable = true
                },
                new MenuItem
                {
                    Id = 9,
                    Name = "Cold Brew",
                    Description = "12 saat soğuk suda demlenmiş pürüzsüz kahve.",
                    Price = 110.00m,
                    Category = "Soğuk İçecekler",
                    IsAvailable = true
                },
                new MenuItem
                {
                    Id = 10,
                    Name = "Frappuccino",
                    Description = "Buzla karıştırılmış, kremalı soğuk kahve tatlısı.",
                    Price = 120.00m,
                    Category = "Soğuk İçecekler",
                    IsAvailable = true
                },
                new MenuItem
                {
                    Id = 11,
                    Name = "Limonata",
                    Description = "Taze sıkılmış limon ve nane yaprakları ile.",
                    Price = 60.00m,
                    Category = "Soğuk İçecekler",
                    IsAvailable = true
                },
                new MenuItem
                {
                    Id = 12,
                    Name = "San Sebastian Cheesecake",
                    Description = "İçi akışkan, üstü yanık enfes İspanyol keki.",
                    Price = 140.00m,
                    Category = "Atıştırmalıklar",
                    IsAvailable = true
                },
                new MenuItem
                {
                    Id = 13,
                    Name = "Brownie",
                    Description = "Yoğun çikolatalı, fıstık parçalı ıslak kek.",
                    Price = 55.00m,
                    Category = "Atıştırmalıklar",
                    IsAvailable = true
                },
                new MenuItem
                {
                    Id = 14,
                    Name = "Avokado Toast",
                    Description = "Tam buğday ekmeği üzerine taze avokado, limon ve kırmızı biber.",
                    Price = 90.00m,
                    Category = "Atıştırmalıklar",
                    IsAvailable = true
                }
            );
        }
    }
}
