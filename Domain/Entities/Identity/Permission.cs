using Domain.Common;
using Domain.Constants;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities.Identity
{
    /// <summary>
    /// موجودیت مجوز (Permission)
    /// هر مجوز نشان‌دهنده یک عملیات خاص در سیستم است (مثلاً orders.view, products.create)
    /// </summary>
    public class Permission : BaseEntity
    {
        /// <summary>
        /// نام مجوز (ساختار سلسله‌مراتبی با نقطه، مثلاً "orders.view")
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// نام نمایشی مجوز (برای نمایش در پنل ادمین)
        /// </summary>
        public string DisplayName { get; private set; } = string.Empty;

        /// <summary>
        /// توضیحات مجوز (اختیاری)
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// ماژول مجوز (برای گروه‌بندی و مدیریت بهتر)
        /// </summary>
        public Module Module { get; private set; }

        /// <summary>
        /// لیست داخلی ارتباط‌های نقش‌هایی که این مجوز را دارند (جدول واسط RolePermission)
        /// </summary>
        private readonly List<RolePermission> _rolePermissions = new();

        /// <summary>
        /// ارتباط‌های نقش‌هایی که این مجوز را دارند (فقط خواندنی)
        /// </summary>
        public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

        /// <summary>
        /// سازنده پیش‌فرض برای EF Core
        /// </summary>
        protected Permission() { }

        /// <summary>
        /// سازنده برای ایجاد مجوز جدید
        /// </summary>
        /// <param name="createBy">شناسه کاربر ایجادکننده</param>
        /// <param name="name">نام مجوز (مثلاً "orders.view")</param>
        /// <param name="displayName">نام نمایشی</param>
        /// <param name="module">ماژول</param>
        /// <param name="description">توضیحات (اختیاری)</param>
        public Permission(
            long createBy,
            string name,
            string displayName,
            Module module,
            string? description = null) : base(createBy)
        {
            InitializePermission(name, displayName, module, description);
        }

        /// <summary>
        /// متد مقداردهی اولیه مجوز با استفاده از متدهای اعتبارسنجی
        /// </summary>
        /// <param name="name">نام مجوز</param>
        /// <param name="displayName">نام نمایشی</param>
        /// <param name="module">ماژول</param>
        /// <param name="description">توضیحات</param>
        private void InitializePermission(string name, string displayName, Module module, string? description)
        {
            SetName(name);
            SetDisplayName(displayName);
            SetModule(module);
            SetDescription(description);
        }

        // ===== متدهای اعتبارسنجی خصوصی =====

        private void SetName(string name)
        {
            var normalized = name?.Trim();
            if (string.IsNullOrWhiteSpace(normalized))
                throw new DomainException("نام مجوز نمی‌تواند خالی باشد.", DomainConcept.Permission, nameof(Name));
            if (Guard.IsInvalidLength(normalized, 3, 100))
                throw new DomainException("نام مجوز باید بین 3 تا 100 کاراکتر باشد.", DomainConcept.Permission, nameof(Name));
            Name = normalized;
        }

        private void SetDisplayName(string displayName)
        {
            var normalized = displayName?.Trim();
            if (string.IsNullOrWhiteSpace(normalized))
                throw new DomainException("نام نمایشی نمی‌تواند خالی باشد.", DomainConcept.Permission, nameof(DisplayName));
            if (Guard.IsInvalidLength(normalized, 3, 100))
                throw new DomainException("نام نمایشی باید بین 3 تا 100 کاراکتر باشد.", DomainConcept.Permission, nameof(DisplayName));
            DisplayName = normalized;
        }

        private void SetModule(Module module)
        {
            if (Guard.IsInvalidEnum(module))
                throw new DomainException("ماژول انتخاب‌شده نامعتبر است.", DomainConcept.Permission, nameof(Module));

            Module = module;
        }

        private void SetDescription(string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                Description = null;
                return;
            }
            var normalized = description.Trim();
            if (Guard.IsOverMaxLength(normalized, 500))
                throw new DomainException("توضیحات نمی‌تواند بیش از 500 کاراکتر باشد.", DomainConcept.Permission, nameof(Description));
            Description = normalized;
        }

        // ===== متدهای به‌روزرسانی عمومی =====

        public void UpdateName(string name, long updateBy)
        {
            SetName(name);
            UpdateModificationInfo(updateBy);
        }

        public void UpdateDisplayName(string displayName, long updateBy)
        {
            SetDisplayName(displayName);
            UpdateModificationInfo(updateBy);
        }

        public void UpdateModule(Module module, long updateBy)
        {
            SetModule(module);
            UpdateModificationInfo(updateBy);
        }

        public void UpdateDescription(string? description, long updateBy)
        {
            SetDescription(description);
            UpdateModificationInfo(updateBy);
        }
    }
}