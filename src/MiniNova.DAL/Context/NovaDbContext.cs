using Microsoft.EntityFrameworkCore;
using MiniNova.DAL.Models;

namespace MiniNova.DAL.Context;

public class NovaDbContext : DbContext
{
    public NovaDbContext(DbContextOptions<NovaDbContext> options) : base(options) { }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<Operator> Operators { get; set; }
    public DbSet<Tracking> Trackings { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Occupation> Occupations { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Size> Sizes { get; set; }
    public DbSet<Status> Statuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) 
    {
        base.OnModelCreating(modelBuilder);
            
        // shpmnt

        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.HasIndex(e => e.TrackId).IsUnique();
            entity.Property(e => e.TrackId).HasMaxLength(17).IsRequired();
            entity.Property(e => e.Weight).HasPrecision(18, 2);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        
        // tracking
        
        modelBuilder.Entity<Tracking>(entity =>
        {
            entity.Property(e => e.UpdateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        
        // decimal numbers
        
        modelBuilder.Entity<Invoice>()
            .Property(i => i.Amount).HasPrecision(18, 2);
            
        modelBuilder.Entity<Operator>()
            .Property(o => o.Salary).HasPrecision(18, 2);

        modelBuilder.Entity<Occupation>()
            .Property(o => o.BaseSalary).HasPrecision(18, 2);
        
        // shpmnt <-> person 
            
        modelBuilder.Entity<Shipment>()
            .HasOne(p => p.Shipper)
            .WithMany(person => person.SentPackages)
            .HasForeignKey(p => p.ShipperId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<Shipment>()
            .HasOne(p => p.Consignee)
            .WithMany(person => person.ReceivedPackages)
            .HasForeignKey(p => p.ConsigneeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // shpmnt <-> location
        
        modelBuilder.Entity<Shipment>()
            .HasOne(p => p.Origin)
            .WithMany()
            .HasForeignKey(p => p.OriginId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Shipment>()
            .HasOne(p => p.Destination)
            .WithMany()
            .HasForeignKey(p => p.DestinationId)
            .OnDelete(DeleteBehavior.Restrict);

        // 1-1
            
        modelBuilder.Entity<Shipment>()
            .HasOne(p => p.Invoice)
            .WithOne(a => a.Shipment)
            .HasForeignKey<Invoice>(i => i.ShipmentId);
            
        modelBuilder.Entity<Person>()
            .HasOne(a => a.Account)
            .WithOne(p => p.Person)
            .HasForeignKey<Account>(a => a.PersonId);

        modelBuilder.Entity<Person>()
            .HasOne(o => o.Operator)
            .WithOne(p => p.Person)
            .HasForeignKey<Operator>(o => o.PersonId);
    }
}