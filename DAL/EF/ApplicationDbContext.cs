using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;

namespace DAL.EF
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<uOrder> uOrders { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Warehouse> Warehouse { get; set; }
        public DbSet<ProductQuantity> ProductQuantity { get; set; }
        public DbSet<whOrder> whOrders { get; set; }
        public ApplicationDbContext()
        {
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\Local;Database=Test");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relationships
            // one to one
            modelBuilder.Entity<Product>()
                .HasOne<ProductQuantity>(p => p.ProductQuantity)
                .WithOne(q => q.Product)
                .HasForeignKey<ProductQuantity>(q => q.ProductId);
            modelBuilder.Entity<uOrder>()
                .HasOne<whOrder>(o => o.whOrder)
                .WithOne(wh => wh.uOrder)
                .HasForeignKey<whOrder>(o => o.uOrderId)
                .OnDelete(DeleteBehavior.NoAction);

            // one to many
            modelBuilder.Entity<whOrder>()
                .HasOne<Product>(p => p.Product)
                .WithMany(o => o.whOrders)
                .HasForeignKey(p => p.ProductId);
            modelBuilder.Entity<uOrder>()
                .HasOne<Product>(p => p.Product)
                .WithMany(o => o.Orders)
                .HasForeignKey(p => p.ProductId);
            modelBuilder.Entity<uOrder>()
                .HasOne<User>(u => u.User)
                .WithMany(o => o.Orders)
                .HasForeignKey(u => u.UserId);
            modelBuilder.Entity<ProductQuantity>()
                .HasOne<Warehouse>(w => w.Warehouse)
                .WithMany(q => q.ProductsQuantity)
                .HasForeignKey(w => w.WarehouseId);


            // Primary data (for Example

            modelBuilder.Entity<Product>()
                .HasData(
                    new Product { Id = 1, Name = "Hammer", Company = "BestHammers", Price = 300 },
                    new Product { Id = 2, Name = "Steel Plate", Company = "Undesteel", Price = 999},
                    new Product { Id = 3, Name = "Wheel", Company = "SmokeDelis", Price = 30 }
                    );
            modelBuilder.Entity<User>()
                .HasData(
                    new User { Id = 1, Login = "admin", Password = "12345", Role = "admin", Date = DateTime.Now }
                );
            modelBuilder.Entity<ProductQuantity>()
                .HasData(
                    new ProductQuantity { Id = 1, ProductId = 1, WarehouseId = 1, Quantity = 100, ReservedQuantity = 0 },
                    new ProductQuantity { Id = 2, ProductId = 2, WarehouseId = 1, Quantity = 50, ReservedQuantity = 0 },
                    new ProductQuantity { Id = 3, ProductId = 3, WarehouseId = 1, Quantity = 10, ReservedQuantity = 0 }
                );
            modelBuilder.Entity<Warehouse>()
                .HasData(
                    new Warehouse { Id = 1, Name = "Warehouse1", Location="BarBar32a" }
                );


        }
    }
}
