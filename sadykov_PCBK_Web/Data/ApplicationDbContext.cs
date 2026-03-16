using Microsoft.EntityFrameworkCore;
using sadykov_PCBK_Web.Models;

namespace sadykov_PCBK_Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Partner>     Partners     { get; set; } = null!;
        public DbSet<PartnerType> PartnerTypes { get; set; } = null!;
        public DbSet<Product>     Products     { get; set; } = null!;
        public DbSet<PartnerSale> PartnerSales { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("app");

            modelBuilder.Entity<PartnerType>(e =>
            {
                e.ToTable("partner_types", "app");
                e.HasIndex(x => x.TypeName).IsUnique();
            });

            modelBuilder.Entity<Partner>(e =>
            {
                e.ToTable("partners", "app");
                e.HasIndex(x => x.Inn).IsUnique();
                e.HasOne(p => p.PartnerType)
                 .WithMany(t => t.Partners)
                 .HasForeignKey(p => p.TypeId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Product>(e =>
            {
                e.ToTable("products", "app");
                e.HasIndex(x => x.Article).IsUnique();
                e.Property(x => x.MinPrice).HasColumnType("numeric(12,2)");
            });

            modelBuilder.Entity<PartnerSale>(e =>
            {
                e.ToTable("partner_sales", "app");
                e.HasOne(s => s.Partner)
                 .WithMany(p => p.Sales)
                 .HasForeignKey(s => s.PartnerId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(s => s.Product)
                 .WithMany(p => p.Sales)
                 .HasForeignKey(s => s.ProductId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
