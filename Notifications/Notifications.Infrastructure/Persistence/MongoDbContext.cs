using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using Notifications.Domain.Entities;

namespace Notifications.Infrastructure.Persistence;

public class MongoDbContext(DbContextOptions<MongoDbContext> options) : DbContext(options)
{
    public DbSet<BindingEntity> Bindings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<BindingEntity>(entity =>
        {
            entity.ToCollection("NotificationBindings");
            
            entity.HasKey(e => e.IdentityId);
            entity.Property(e => e.IdentityId)
                .HasElementName("_id")
                .ValueGeneratedOnAdd();
            
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);
        });
    }
}