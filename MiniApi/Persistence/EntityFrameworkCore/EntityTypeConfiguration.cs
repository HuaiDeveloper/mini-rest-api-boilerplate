using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MiniApi.Model;

namespace MiniApi.Persistence.EntityFrameworkCore;

public class ProductConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Product");

        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
    }
}

public class StaffConfig : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.ToTable("Staff");

        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

    }
}

public class CurrentPriceConfig : IEntityTypeConfiguration<CurrentPrice>
{
    public void Configure(EntityTypeBuilder<CurrentPrice> builder)
    {
        builder.ToTable("CurrentPrice");

        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.HasOne(c => c.Product)
            .WithMany(p => p.CurrentPrices)
            .HasForeignKey(c => c.ProductId)
            .OnDelete(DeleteBehavior.ClientCascade);

    }
}

public class SerialNumberCodeConfig : IEntityTypeConfiguration<SerialNumberCode>
{
    public void Configure(EntityTypeBuilder<SerialNumberCode> builder)
    {
        builder.ToTable("SerialNumberCode");

        builder.HasKey(x => x.Id);
    }
}
