using Microsoft.EntityFrameworkCore;
using MesClient.Core.Models;

namespace MesClient.Infrastructure.Data;

/// <summary>
/// MES 데이터베이스 컨텍스트
/// </summary>
public class MesDbContext : DbContext
{
    public MesDbContext(DbContextOptions<MesDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Equipment> Equipments => Set<Equipment>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<Production> Productions => Set<Production>();
    public DbSet<QualityInspection> QualityInspections => Set<QualityInspection>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Alarm> Alarms => Set<Alarm>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.Property(e => e.UserId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.UserName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(200);
        });

        // Equipment
        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.EquipmentCode).IsUnique();
            entity.Property(e => e.EquipmentCode).HasMaxLength(50).IsRequired();
            entity.Property(e => e.EquipmentName).HasMaxLength(200).IsRequired();
        });

        // WorkOrder
        modelBuilder.Entity<WorkOrder>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.WorkOrderNo).IsUnique();
            entity.Property(e => e.WorkOrderNo).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.PlanStartTime);
            entity.HasIndex(e => e.Status);
        });

        // Production
        modelBuilder.Entity<Production>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProductionNo).IsUnique();
            entity.Property(e => e.ProductionNo).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.WorkOrderNo);
            entity.HasIndex(e => e.ProductionTime);
        });

        // QualityInspection
        modelBuilder.Entity<QualityInspection>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.InspectionNo).IsUnique();
            entity.Property(e => e.InspectionNo).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.LotNo);
        });

        // Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProductCode).IsUnique();
            entity.Property(e => e.ProductCode).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ProductName).HasMaxLength(200).IsRequired();
        });

        // Alarm
        modelBuilder.Entity<Alarm>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.OccurredAt);
            entity.HasIndex(e => e.EquipmentCode);
        });
    }
}
