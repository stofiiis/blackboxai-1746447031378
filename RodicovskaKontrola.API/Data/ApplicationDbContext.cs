using Microsoft.EntityFrameworkCore;
using RodicovskaKontrola.API.Models;

namespace RodicovskaKontrola.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ChildStatus> ChildStatuses { get; set; }
    public DbSet<ControlCommand> ControlCommands { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ChildStatus>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ChildId).IsRequired();
            entity.Property(e => e.ActiveProcesses).IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();
        });

        modelBuilder.Entity<ControlCommand>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ChildId).IsRequired();
            entity.Property(e => e.CommandType).IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.Status).IsRequired();
        });
    }
}
