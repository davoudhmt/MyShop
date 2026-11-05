using Domain.Common;
using Domain.Constants;
using Domain.Exceptions;

namespace Domain.Entities.Identity
{
    /// <summary>
    /// موجودیت نقش (Role)
    /// هر نقش مجموعه‌ای از مجوزها (Permissions) را در بر می‌گیرد و به کاربران تخصیص داده می‌شود
    /// </summary>
    public class Role : BaseEntity
    {
        /// <summary>
        /// نام نقش (باید یونیک باشد، مثلاً "Admin", "Customer", "OrderManager")
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// توضیحات نقش (برای نقش‌های غیرسیستمی اجباری است)
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// آیا این نقش یک نقش سیستمی است؟
        /// </summary>
        public bool IsSystem { get; private set; } = false;

        /// <summary>
        /// لیست داخلی کاربرانی که این نقش را دارند
        /// </summary>
        private readonly List<UserRole> _userRoles = new();

        /// <summary>
        /// کاربرانی که این نقش را دارند (فقط خواندنی)
        /// </summary>
        public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

        /// <summary>
        /// لیست داخلی ارتباط‌های مجوز این نقش (جدول واسط RolePermission)
        /// </summary>
        private readonly List<RolePermission> _rolePermissions = new();

        /// <summary>
        /// ارتباط‌های مجوز تخصیص داده شده به این نقش (فقط خواندنی)
        /// </summary>
        public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

        /// <summary>
        /// سازنده پیش‌فرض برای EF Core
        /// </summary>
        protected Role() { }

        /// <summary>
        /// سازنده برای ایجاد نقش جدید
        /// </summary>
        /// <param name="createBy">شناسه کاربر ایجادکننده</param>
        /// <param name="name">نام نقش</param>
        /// <param name="isSystem">آیا نقش سیستمی است؟ (پیش‌فرض: false)</param>
        /// <param name="description">توضیحات نقش (برای نقش‌های غیرسیستمی اجباری)</param>
        public Role(
            long createBy,
            string name,
            bool isSystem = false,
            string? description = null) : base(createBy)
        {
            InitializeRole(name, isSystem, description);
        }

        /// <summary>
        /// متد مقداردهی اولیه نقش با استفاده از متدهای اعتبارسنجی
        /// </summary>
        /// <param name="name">نام نقش</param>
        /// <param name="isSystem">آیا نقش سیستمی است؟</param>
        /// <param name="description">توضیحات نقش</param>
        private void InitializeRole(string name, bool isSystem, string? description)
        {
            SetName(name);
            SetIsSystem(isSystem);
            SetDescription(description);
        }

        // ===== متدهای اعتبارسنجی خصوصی =====

        /// <summary>
        /// تنظیم و اعتبارسنجی نام نقش
        /// </summary>
        /// <param name="name">نام ورودی</param>
        /// <exception cref="DomainException">در صورت خالی بودن یا طول نامعتبر</exception>
        private void SetName(string name)
        {
            var normalized = name?.Trim();

            if (string.IsNullOrWhiteSpace(normalized))
                throw new DomainException("نام نقش نمی‌تواند خالی باشد.", DomainConcept.Role, nameof(Name));

            if (Guard.IsInvalidLength(normalized, 2, 50))
                throw new DomainException("نام نقش باید بین 2 تا 50 کاراکتر باشد.", DomainConcept.Role, nameof(Name));

            Name = normalized;
        }

        /// <summary>
        /// تنظیم و اعتبارسنجی وضعیت سیستمی بودن نقش
        /// </summary>
        /// <param name="isSystem">آیا نقش سیستمی است؟</param>
        private void SetIsSystem(bool isSystem)
        {
            IsSystem = isSystem;
        }

        /// <summary>
        /// تنظیم و اعتبارسنجی توضیحات نقش
        /// اگر نقش سیستمی نباشد (IsSystem = false)، توضیحات اجباری است
        /// </summary>
        /// <param name="description">توضیحات ورودی</param>
        /// <exception cref="DomainException">در صورت خالی بودن (برای نقش‌های غیرسیستمی) یا طول نامعتبر</exception>
        private void SetDescription(string? description)
        {
            // اگر نقش غیرسیستمی است، توضیحات اجباری است
            if (!IsSystem && string.IsNullOrWhiteSpace(description))
                throw new DomainException("برای نقش‌های غیرسیستمی، توضیحات اجباری است.", DomainConcept.Role, nameof(Description));

            // اگر توضیحات خالی است (و نقش سیستمی است)، null قرار می‌دهیم
            if (string.IsNullOrWhiteSpace(description))
            {
                Description = null;
                return;
            }

            var normalized = description.Trim();

            if (Guard.IsOverMaxLength(normalized, 500))
                throw new DomainException("توضیحات نقش نمی‌تواند بیش از 500 کاراکتر باشد.", DomainConcept.Role, nameof(Description));

            Description = normalized;
        }

        // ===== متدهای به‌روزرسانی عمومی =====

        /// <summary>
        /// به‌روزرسانی نام نقش
        /// </summary>
        /// <param name="name">نام جدید</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UpdateName(string name, long updateBy)
        {
            SetName(name);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// به‌روزرسانی توضیحات نقش
        /// </summary>
        /// <param name="description">توضیحات جدید (اختیاری برای نقش‌های سیستمی، اجباری برای غیرسیستمی)</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UpdateDescription(string? description, long updateBy)
        {
            SetDescription(description);
            UpdateModificationInfo(updateBy);
        }

        // ===== مدیریت مجوزها =====

        /// <summary>
        /// افزودن مجوز به این نقش
        /// </summary>
        /// <param name="permission">مجوز مورد نظر</param>
        /// <param name="createBy">شناسه کاربر ایجادکننده</param>
        public void AddPermission(Permission permission, long createBy)
        {
            if (permission is null)
                throw new DomainException("مجوز نمی‌تواند خالی باشد.", DomainConcept.Role, nameof(RolePermissions));

            // جلوگیری از افزودن تکراری
            if (_rolePermissions.Any(rp => rp.PermissionId == permission.Id))
                return;

            // ایجاد رکورد در جدول واسط
            var rolePermission = new RolePermission(createBy, this.Id, permission.Id);
            _rolePermissions.Add(rolePermission);
        }

        /// <summary>
        /// حذف مجوز از این نقش
        /// </summary>
        /// <param name="permissionId">شناسه مجوز مورد نظر</param>
        public void RemovePermission(long permissionId)
        {
            var rolePermission = _rolePermissions.FirstOrDefault(rp => rp.PermissionId == permissionId);
            if (rolePermission != null)
            {
                _rolePermissions.Remove(rolePermission);
            }
        }

        /// <summary>
        /// پاکسازی تمام مجوزهای این نقش
        /// </summary>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void ClearPermissions(long updateBy)
        {
            _rolePermissions.Clear();
            UpdateModificationInfo(updateBy);
        }
    }
}