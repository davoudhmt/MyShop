using Domain.Common;
using Domain.Constants;
using Domain.Exceptions;

namespace Domain.Entities.Identity
{
    /// <summary>
    /// موجودیت ارتباطی بین Role و Permission (جدول N:N)
    /// هر رکورد نشان‌دهنده تخصیص یک مجوز به یک نقش است
    /// </summary>
    public class RolePermission : BaseEntity
    {
        /// <summary>
        /// کلید خارجی به Role
        /// </summary>
        public long RoleId { get; private set; }

        /// <summary>
        /// خاصیت ناوبری به Role
        /// </summary>
        public Role Role { get; private set; } = null!;

        /// <summary>
        /// کلید خارجی به Permission
        /// </summary>
        public long PermissionId { get; private set; }

        /// <summary>
        /// خاصیت ناوبری به Permission
        /// </summary>
        public Permission Permission { get; private set; } = null!;

        /// <summary>
        /// سازنده پیش‌فرض برای EF Core
        /// </summary>
        protected RolePermission() { }

        /// <summary>
        /// سازنده برای ایجاد ارتباط جدید بین Role و Permission
        /// </summary>
        /// <param name="createBy">شناسه کاربر ایجادکننده</param>
        /// <param name="roleId">شناسه Role</param>
        /// <param name="permissionId">شناسه Permission</param>
        public RolePermission(long createBy, long roleId, long permissionId) : base(createBy)
        {
            InitializeRolePermission(roleId, permissionId);
        }

        /// <summary>
        /// متد مقداردهی اولیه با استفاده از متدهای اعتبارسنجی
        /// </summary>
        /// <param name="roleId">شناسه Role</param>
        /// <param name="permissionId">شناسه Permission</param>
        private void InitializeRolePermission(long roleId, long permissionId)
        {
            SetRoleId(roleId);
            SetPermissionId(permissionId);
        }

        // ===== متدهای اعتبارسنجی خصوصی =====

        /// <summary>
        /// تنظیم و اعتبارسنجی شناسه Role
        /// </summary>
        /// <param name="roleId">شناسه Role</param>
        /// <exception cref="DomainException">در صورت نامعتبر بودن</exception>
        private void SetRoleId(long roleId)
        {
            if (Guard.IsInvalidId(roleId))
                throw new DomainException("شناسه نقش نامعتبر است.", DomainConcept.RolePermission, nameof(RoleId));

            RoleId = roleId;
        }

        /// <summary>
        /// تنظیم و اعتبارسنجی شناسه Permission
        /// </summary>
        /// <param name="permissionId">شناسه Permission</param>
        /// <exception cref="DomainException">در صورت نامعتبر بودن</exception>
        private void SetPermissionId(long permissionId)
        {
            if (Guard.IsInvalidId(permissionId))
                throw new DomainException("شناسه مجوز نامعتبر است.", DomainConcept.RolePermission, nameof(PermissionId));

            PermissionId = permissionId;
        }
    }
}