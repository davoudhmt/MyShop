using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Persistence.Configurations
{
    /// <summary>
    /// کلاس پایه برای کانفیگ موجودیت‌های دارای BaseEntity
    /// </summary>
    /// <typeparam name="T">نوع موجودیت که از BaseEntity ارث می‌برد</typeparam>
    public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        /// <summary>
        /// متد کانفیگ فیلدهای پایه موجودیت
        /// </summary>
        /// <param name="builder">سازنده نوع موجودیت</param>
        protected void ConfigureBaseEntity(EntityTypeBuilder<T> builder)
        {
            // ===== کلید اصلی =====
            builder.HasKey(e => e.Id);

            // ===== فیلد CreateBy =====
            builder.Property(e => e.CreateBy)
                .IsRequired();

            // ===== فیلد CreateDate =====
            builder.Property(e => e.CreateDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // ===== فیلد UpdateBy =====
            builder.Property(e => e.UpdateBy)
                .IsRequired(false);

            // ===== فیلد UpdateDate =====
            builder.Property(e => e.UpdateDate)
                .IsRequired(false);

            // ===== فیلد Status =====
            builder.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            // ===== فیلد RowVersion =====
            builder.Property(e => e.RowVersion)
                .IsRowVersion();

            // ===== فیلدهای محاسبه‌ای (ذخیره نمی‌شن) =====
            builder.Ignore(e => e.IsDeleted);
            builder.Ignore(e => e.IsActive);
        }

        /// <summary>
        /// متد کانفیگ اصلی (هر کلاس کانفیگ باید پیاده کنه)
        /// </summary>
        /// <param name="builder">سازنده نوع موجودیت</param>
        public abstract void Configure(EntityTypeBuilder<T> builder);
    }
}