using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infra.Persistence.Configurations;

namespace Infra.Persistence.Configurations.Identity
{
    /// <summary>
    /// کلاس پیکربندی موجودیت شخص (Person) در Entity Framework Core
    /// این کلاس قوانین نگاشت به دیتابیس، شامل نام جدول، محدودیت‌های فیلدها، ایندکس‌ها، 
    /// روابط ارجاعی و پیکربندی Value Objectها را تعریف می‌کند.
    /// </summary>
    public class PersonConfiguration : BaseEntityConfiguration<Person>
    {
        /// <summary>
        /// اعمال قوانین پیکربندی بر روی موجودیت Person
        /// </summary>
        /// <param name="builder">سازنده‌ی موجودیت (Entity Type Builder) برای اعمال تنظیمات EF Core</param>
        public override void Configure(EntityTypeBuilder<Person> builder)
        {
            // ۱. تنظیم نام جدول و اسکیما
            builder.ToTable("Persons", "Identity");

            // ۲. پیکربندی فیلدهای ساده و ایندکس‌ها
            builder.Property(p => p.FirstName)
                .IsRequired(false)
                .HasMaxLength(100)
                .IsUnicode(true);

            builder.Property(p => p.LastName)
                .IsRequired(false)
                .HasMaxLength(100)
                .IsUnicode(true);

            builder.Property(p => p.UserId)
                .IsRequired(false);

            builder.HasIndex(p => p.UserId)
                .IsUnique()
                .HasDatabaseName("UX_Persons_UserId");

            // ۳. پیکربندی Value Objectها (Owned Types)

            // موبایل (اجباری و یونیک - کلید اصلی برای ورود)
            builder.OwnsOne(p => p.Mobile, mobileBuilder => {
                mobileBuilder.Property(m => m.Value)
                    .HasColumnName("Mobile")
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false);

                mobileBuilder.HasIndex(m => m.Value)
                    .IsUnique()
                    .HasDatabaseName("UX_Persons_Mobile");
            });

            // ایمیل (اختیاری و یونیک)
            builder.OwnsOne(p => p.Email, emailBuilder => {
                emailBuilder.Property(e => e.Value)
                    .HasColumnName("Email")
                    .IsRequired(false)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                emailBuilder.HasIndex(e => e.Value)
                    .IsUnique()
                    .HasDatabaseName("UX_Persons_Email");
            });

            // تلفن ثابت (اختیاری)
            builder.OwnsOne(p => p.Landline, landlineBuilder => {
                landlineBuilder.Property(l => l.Value)
                    .HasColumnName("Landline")
                    .IsRequired(false)
                    .HasMaxLength(11)
                    .IsUnicode(false);
            });

            // ۴. پیکربندی روابط (Relationships)

            // رابطه با User (یک به یک یا صفر)
            builder.HasOne(p => p.User)
                .WithOne(u => u.Person)
                .HasForeignKey<User>(u => u.PersonId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // ۵. اعمال پیکربندی‌های پایه (مانند Id، تاریخ ایجاد، Status و RowVersion)
            ConfigureBaseEntity(builder);
        }
    }
}