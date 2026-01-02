using API.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace API.DAL.Context;

public class NovaDbContext : DbContext
{
    public NovaDbContext(DbContextOptions<NovaDbContext> options) : base(options) { }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Package> Packages { get; set; }
    public DbSet<Destination> Destinations { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<Operator> Operators { get; set; }
    public DbSet<Tracking> Trackings { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Occupation> Occupations { get; set; }
    public DbSet<Invoice> Invoices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) 
    {
        base.OnModelCreating(modelBuilder);
            
        // pkg <-> person 
            
        modelBuilder.Entity<Package>()
            .HasOne(p => p.Sender)
            .WithMany(person => person.SentPackages)
            .HasForeignKey(p => p.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<Package>()
            .HasOne(p => p.Receiver)
            .WithMany(person => person.ReceivedPackages)
            .HasForeignKey(p => p.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        // 1-1
            
        modelBuilder.Entity<Package>()
            .HasOne(p => p.Invoice)
            .WithOne(a => a.Package)
            .HasForeignKey<Invoice>(i => i.PackageId);
            
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