using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(u => u.CustomerId).HasColumnType("uuid").HasColumnName("CustomerId");
        builder.Property(u => u.BranchId).HasColumnType("uuid").HasColumnName("BranchId");

        builder.HasMany(u => u.Items)
           .WithOne(i => i.Sale)
           .HasForeignKey(i => i.SaleId)
           .OnDelete(DeleteBehavior.Cascade);
    }
}
