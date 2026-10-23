using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infra.Persistence.Configurations;

namespace Infra.Persistence.Configurations.Identity
{
    /// <summary>
    /// کلاس پیکربندی موجودیت آدرس (Address) در Entity Framework Core
    /// این کلاس قوانین نگاشت به دیتابیس برای آدرس‌های کاربر، Value Object کد پستی و روابط را تعریف می‌کند.
    /// </summary>
    public class AddressConfiguration : BaseEntityConfiguration<Address>
    {
        /// <summary>
        /// اعمال قوانین پیکربندی بر روی موجودیت Address
        /// </summary>
        /// <param name="builder">سازنده‌ی موجودیت (Entity Type Builder) برای اعمال تنظیمات EF Core</param>
        public override void Configure(EntityTypeBuilder<Address> builder)
        {
            // ۱. تنظیم نام جدول و اسکیما
            builder.ToTable("Addresses", "Identity");

            // ۲. پیکربندی فیلدهای ساده
            builder.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(true);

            builder.Property(a => a.AddressDetail)
                .IsRequired()
                .HasMaxLength(500)
                .IsUnicode(true);

            builder.Property(a => a.IsDefault)
                .IsRequired()
                .HasDefaultValue(false);

            // ۳. پیکربندی Value Object (Owned Type)
            // کد پستی به صورت یک ستون مستقل با نام PostalCode در جدول ذخیره می‌شود
            builder.OwnsOne(a => a.PostalCode, pc => {
                pc.Property(p => p.Value)
                    .HasColumnName("PostalCode")
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false); // کد پستی فقط عدد است

                pc.HasIndex(p => p.Value)
                    .HasDatabaseName("IX_Addresses_PostalCode");
            });

            // ۴. پیکربندی کلیدهای خارجی و ایندکس‌ها
            builder.Property(a => a.RegionId)
                .IsRequired();

            builder.HasIndex(a => a.RegionId)
                .HasDatabaseName("IX_Addresses_RegionId");

            builder.Property(a => a.PersonId)
                .IsRequired();

            builder.HasIndex(a => a.PersonId)
                .HasDatabaseName("IX_Addresses_PersonId");

            // ۵. پیکربندی روابط ارجاعی
            // رابطه با Region (چند به یک)
            builder.HasOne(a => a.Region)
                .WithMany() // چون لیست Address در کلاس Region نداریم
                .HasForeignKey(a => a.RegionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // جلوگیری از حذف شهر اگر آدرسی به آن متصل است

            // رابطه با Person (چند به یک)
            builder.HasOne(a => a.Person)
                .WithMany() // چون لیست Address در کلاس Person نداریم (طبق توافق قبلی)
                .HasForeignKey(a => a.PersonId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // جلوگیری از حذف شخص اگر آدرسی به آن متصل است

            // ۶. اعمال پیکربندی‌های پایه (مانند Id، تاریخ ایجاد، Status و RowVersion)
            ConfigureBaseEntity(builder);
        }
    }
}