using Microsoft.EntityFrameworkCore;
using NameService.Models;


namespace NameService.Data;

public class NameDbContext : DbContext//this is database initialization class
{
    public NameDbContext(DbContextOptions<NameDbContext> options)
        : base(options)
    {
    }

    public DbSet<Name> Names { get; set; }//table
    public DbSet<Product> Products { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, ProductName = "Bag", Brand = "Nike" },
            new Product { Id = 2, ProductName = "Toy", Brand = "Lego" },
            new Product { Id = 3, ProductName = "Stationery", Brand = "Classmate" }
        );
    }
}