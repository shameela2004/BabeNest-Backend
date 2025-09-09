using Microsoft.EntityFrameworkCore;
using BabeNest_Backend.Entities;

namespace BabeNest_Backend.Data
{
    public class BabeNestDbContext: DbContext
    {
        public BabeNestDbContext(DbContextOptions<BabeNestDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet <Cart> Carts { get; set; }
        public DbSet <Wishlist > Wishlists { get; set; }
        public DbSet <Order> Orders { get; set; }
        public DbSet <OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Address> Addresses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Category>().HasData(
       new Category { Id = 1, Name = "Baby Care" },
       new Category { Id = 2, Name = "Toys" },
       new Category { Id = 3, Name = "Clothing" },
       new Category { Id = 4, Name = "Feeding" }
   );


            modelBuilder.Entity<Product>().HasData(
    new Product
    {
        Id = 1,
        Name = "Baby Shampoo",
        Description = "Gentle shampoo for babies",
        Price = 199.99M,
        Stock = 50,
        Image = "images/products/baby-shampoo.jpg",
        CategoryId = 1
    },
    new Product
    {
        Id = 2,
        Name = "Soft Teddy Bear",
        Description = "Fluffy teddy bear toy for infants",
        Price = 499.00M,
        Stock = 20,
        Image = "images/products/teddy-bear.jpg",
        CategoryId = 2
    },
    new Product
    {
        Id = 3,
        Name = "Baby Onesie",
        Description = "Cotton onesie for newborns",
        Price = 299.50M,
        Stock = 100,
        Image = "images/products/baby-onesie.jpg",
        CategoryId = 3
    },
    new Product
    {
        Id = 4,
        Name = "Feeding Bottle",
        Description = "Anti-colic feeding bottle",
        Price = 150.00M,
        Stock = 75,
        Image = "images/products/feeding-bottle.jpg",
        CategoryId = 4
    }
);
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1000,
                Username = "AdminUser",
                Email = "admin@babenest.com",
                PasswordHash = "$2a$11$uJX3yK5E6M/T7bG.Z1Lr6e3N6W85i/7gpoZkiy3vMb/n1/0U4hYF2", // precomputed static hash
                Role = "Admin",
                Blocked = false,
                CreatedAt = new DateTime(2025, 9, 8, 12, 0, 0)
            });




            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany(u => u.Carts)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany(u => u.Wishlists)
                .HasForeignKey(w => w.UserId);

            modelBuilder.Entity<Product>()
                .HasOne(p=>p.Category)
                .WithMany(c=>c.Products)
                .HasForeignKey(p=>p.CategoryId);

            modelBuilder.Entity<Review>()
               .HasOne(r => r.User)
               .WithMany(u => u.Reviews)
               .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<Review>()
                         .HasOne(r => r.Product)
                         .WithMany(p => p.Reviews)
                         .HasForeignKey(r => r.ProductId);

              modelBuilder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
