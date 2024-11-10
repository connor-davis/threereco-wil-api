using Microsoft.EntityFrameworkCore;
using three_api.Lib.Models;

namespace three_api.Lib.Database
{
    public class DatabaseContext(
        DbContextOptions<DatabaseContext> dbContextOptions
    ) : DbContext(dbContextOptions)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<Collector> Collectors { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Collection> Collections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .ToTable("Users")
                .HasKey(User => User.Id);
            modelBuilder.Entity<Business>()
                .ToTable("Businesses")
                .HasKey(Business => Business.Id);
            modelBuilder.Entity<Collector>()
                .ToTable("Collectors")
                .HasKey(Collector => Collector.Id);
            modelBuilder.Entity<Product>()
                .ToTable("Products")
                .HasKey(Product => Product.Id);
            modelBuilder.Entity<Collection>()
                .ToTable("Collections")
                .HasKey(Collection => Collection.Id);

            modelBuilder.Entity<Business>()
                .HasOne(Business => Business.User)
                .WithOne()
                .HasForeignKey<Business>(Business => Business.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Collector>()
                .HasOne(Collector => Collector.User)
                .WithOne()
                .HasForeignKey<Collector>(Collector => Collector.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .HasOne(Product => Product.Business)
                .WithOne()
                .HasForeignKey<Product>(Product => Product.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Collection>()
                .HasOne(Collection => Collection.Business)
                .WithOne()
                .HasForeignKey<Collection>(Collection => Collection.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Collection>()
                .HasOne(Collection => Collection.Product)
                .WithOne()
                .HasForeignKey<Collection>(Collection => Collection.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Collection>()
                .HasOne(Collection => Collection.Collector)
                .WithOne()
                .HasForeignKey<Collection>(Collection => Collection.CollectorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
