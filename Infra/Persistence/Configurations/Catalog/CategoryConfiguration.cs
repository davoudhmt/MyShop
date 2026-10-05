using Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Persistence.Configurations.Catalog
{
    public class CategoryConfiguration : BaseEntityConfiguration<Category>
    {
        public override void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories", "Catalog");

            builder.Property(c => c.Name).IsRequired().HasMaxLength(100).IsUnicode(true);
            builder.HasIndex(c => c.Name).IsUnique().HasDatabaseName("UX_Categories_Name");

            builder.Property(c => c.Description).IsRequired(false).HasMaxLength(500).IsUnicode(true);

            builder.OwnsOne(c => c.CategoryCode, code => {
                code.Property(cc => cc.Value).HasColumnName("CategoryCode").IsRequired().HasMaxLength(10).IsUnicode(false);
                code.HasIndex(cc => cc.Value).IsUnique().HasDatabaseName("UX_Categories_CategoryCode");
            });

            builder.OwnsOne(c => c.ImageUrl, imageUrl => {
                imageUrl.Property(i => i.Value).HasColumnName("ImageUrl").IsRequired(false).HasMaxLength(500).IsUnicode(true);
            });

            builder.HasOne<Category>(c => c.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentCategoryId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(c => c.ParentCategoryId).HasDatabaseName("IX_Categories_ParentCategoryId");

            // ===== رابطه چند‌به‌چند با ProductType =====
            builder.HasMany(c => c.ProductTypes)
                .WithMany(pt => pt.Categories)
                .UsingEntity<Dictionary<string, object>>(
                    "CategoryProductTypes",
                    right => right.HasOne<ProductType>().WithMany().HasForeignKey("ProductTypeId").OnDelete(DeleteBehavior.Cascade),
                    left => left.HasOne<Category>().WithMany().HasForeignKey("CategoryId").OnDelete(DeleteBehavior.Restrict), // ✅ تغییر به Restrict
                    join => {
                        join.ToTable("CategoryProductTypes", "Catalog");
                        join.HasKey("CategoryId", "ProductTypeId");
                        join.HasIndex("ProductTypeId").HasDatabaseName("IX_CategoryProductTypes_ProductTypeId");
                    });

            ConfigureBaseEntity(builder);
        }
    }
}