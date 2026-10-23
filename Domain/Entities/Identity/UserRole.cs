using Domain.Common;
using Domain.Constants;
using Domain.Exceptions;

namespace Domain.Entities.Identity
{
    /// <summary>
    /// موجودیت ارتباطی بین User و Role (جدول N:N)
    /// هر رکورد نشان‌دهنده تخصیص یک نقش به یک کاربر است
    /// </summary>
    public class UserRole : BaseEntity
    {
        /// <summary>
        /// کلید خارجی به User
        /// </summary>
        public long UserId { get; private set; }

        /// <summary>
        /// خاصیت ناوبری به User
        /// </summary>
        public User User { get; private set; } = null!;

        /// <summary>
        /// کلید خارجی به Role
        /// </summary>
        public long RoleId { get; private set; }

        /// <summary>
        /// خاصیت ناوبری به Role
        /// </summary>
        public Role Role { get; private set; } = null!;

        /// <summary>
        /// سازنده پیش‌فرض برای EF Core
        /// </summary>
        protected UserRole() { }

        /// <summary>
        /// سازنده برای ایجاد ارتباط جدید بین User و Role
        /// </summary>
        /// <param name="createBy">شناسه کاربر ایجادکننده</param>
        /// <param name="userId">شناسه User</param>
        /// <param name="roleId">شناسه Role</param>
        public UserRole(long createBy, long userId, long roleId) : base(createBy)
        {
            InitializeUserRole(userId, roleId);
        }

        /// <summary>
        /// متد مقداردهی اولیه با استفاده از متدهای اعتبارسنجی
        /// </summary>
        /// <param name="userId">شناسه User</param>
        /// <param name="roleId">شناسه Role</param>
        private void InitializeUserRole(long userId, long roleId)
        {
            SetUserId(userId);
            SetRoleId(roleId);
        }

        // ===== متدهای اعتبارسنجی خصوصی =====

        /// <summary>
        /// تنظیم و اعتبارسنجی شناسه User
        /// </summary>
        /// <param name="userId">شناسه User</param>
        /// <exception cref="DomainException">در صورت نامعتبر بودن</exception>
        private void SetUserId(long userId)
        {
            if (Guard.IsInvalidId(userId))
                throw new DomainException("شناسه کاربر نامعتبر است.", DomainConcept.UserRole, nameof(UserId));

            UserId = userId;
        }

        /// <summary>
        /// تنظیم و اعتبارسنجی شناسه Role
        /// </summary>
        /// <param name="roleId">شناسه Role</param>
        /// <exception cref="DomainException">در صورت نامعتبر بودن</exception>
        private void SetRoleId(long roleId)
        {
            if (Guard.IsInvalidId(roleId))
                throw new DomainException("شناسه نقش نامعتبر است.", DomainConcept.UserRole, nameof(RoleId));

            RoleId = roleId;
        }
    }
}