using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Persistence.Configurations.Catalog
{
    public class TagConfiguration : BaseEntityConfiguration<Tag>
    {
        public override void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("Tags", "Catalog");

            builder.Property(t => t.Name).IsRequired().HasMaxLength(50).IsUnicode(true);
            builder.HasIndex(t => t.Name).IsUnique().HasDatabaseName("UX_Tags_Name");

            // ===== رابطه چند‌به‌چند با Product =====
            builder.HasMany(t => t.Products)
                .WithMany(p => p.Tags)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductTags",
                    right => right.HasOne<Product>().WithMany().HasForeignKey("ProductId").OnDelete(DeleteBehavior.Cascade),
                    left => left.HasOne<Tag>().WithMany().HasForeignKey("TagId").OnDelete(DeleteBehavior.Restrict), // ✅ تغییر به Restrict
                    join => {
                        join.ToTable("ProductTags", "Catalog");
                        join.HasKey("ProductId", "TagId");
                        join.HasIndex("TagId").HasDatabaseName("IX_ProductTags_TagId");
                    });

            ConfigureBaseEntity(builder);
        }
    }
}