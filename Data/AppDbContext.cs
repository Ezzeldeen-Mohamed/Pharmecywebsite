using ePharma_asp_mvc.Models;
using ePharma_asp_mvc.Models.Static;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ePharma_asp_mvc.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<HealthService> HealthServices { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // هنا ممكن نحط HasData للجداول الثابتة (اختياري)
            modelBuilder.Entity<HealthService>().HasData(
                new HealthService { Id = 1, Name = "قياس ضغط وسكر", Price = 20, Description = "خدمة منزلية" },
                new HealthService { Id = 2, Name = "إعطاء حقنة عضل/وريد", Price = 10, Description = "زيارة منزلية" }
            );

            modelBuilder.Entity<Prescription>()
                .Property(p => p.Status)
                .HasConversion<string>();
        }

        // دالة Seeding جديدة
        public async Task SeedDatabaseAsync(IServiceProvider serviceProvider)
        {
            await Database.MigrateAsync();

            // Seed Products
            if (!Products.Any())
            {
                Products.AddRange(
                    new Product
                    {
                        Name = "Bioderma",
                        ImageUrl = "/images/product_01.png",
                        Price = 7m,
                        Stock = 100
                    },
                    new Product
                    {
                        Name = "Chanca Pedra",
                        ImageUrl = "/images/product_02.png",
                        Price = 11m,
                        Stock = 50
                    }
                );
                await SaveChangesAsync();
            }

            // Seed Identity Roles + Users
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Roles
            if (!await roleManager.RoleExistsAsync(UsersRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(UsersRoles.Admin));

            if (!await roleManager.RoleExistsAsync(UsersRoles.User))
                await roleManager.CreateAsync(new IdentityRole(UsersRoles.User));

            // Admin
            var adminEmail = "admin@gmail.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin-ezz",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    PhoneNumber = "01000000000",
                    Address = "Cairo"
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, UsersRoles.Admin);
            }

            // Normal User
            var userEmail = "ezz@gmail.com";
            if (await userManager.FindByEmailAsync(userEmail) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "ezzeldeen",
                    Email = userEmail,
                    EmailConfirmed = true,
                    FirstName = "Ezzeldeen",
                    PhoneNumber = "01151282446",
                    Address = "Cairo"
                };

                var result = await userManager.CreateAsync(user, "Ezz@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, UsersRoles.User);
            }
        }
    }
}