using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MiniApi.Domain;

namespace MiniApi.Persistence.EntityFrameworkCore
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder
                .ToTable("Product");

        }
    }
}
