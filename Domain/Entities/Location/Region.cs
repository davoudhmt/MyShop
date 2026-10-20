using Domain.Common;
using Domain.Constants;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities.Location
{
    /// <summary>
    /// موجودیت منطقه (تقسیمات کشوری شامل استان و شهر)
    /// این کلاس به صورت Self-Referencing طراحی شده است (هر شهر یک ParentId به استان دارد)
    /// </summary>
    public class Region : BaseEntity
    {
        /// <summary>
        /// نام منطقه (نام استان یا نام شهر)
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// نوع منطقه (استان یا شهر)
        /// </summary>
        public RegionType Type { get; private set; }

        /// <summary>
        /// شناسه منطقه پدر (برای شهرها، این فیلد شناسه استان مادر است. برای استان‌ها null است)
        /// </summary>
        public long? ParentId { get; private set; }

        /// <summary>
        /// خاصیت ناوبری به منطقه پدر (اختیاری)
        /// </summary>
        public Region? Parent { get; private set; }

        /// <summary>
        /// لیست داخلی مناطق زیرمجموعه (فرزندان)
        /// </summary>
        private readonly List<Region> _children = new();

        /// <summary>
        /// مناطق زیرمجموعه (فقط خواندنی)
        /// </summary>
        public IReadOnlyCollection<Region> Children => _children.AsReadOnly();

        /// <summary>
        /// سازنده پیش‌فرض برای EF Core
        /// </summary>
        protected Region() { }

        /// <summary>
        /// سازنده برای ایجاد منطقه جدید
        /// </summary>
        /// <param name="createBy">شناسه کاربر ایجادکننده</param>
        /// <param name="name">نام منطقه</param>
        /// <param name="type">نوع منطقه (استان یا شهر)</param>
        /// <param name="parentId">شناسه منطقه پدر (اختیاری، برای شهرها الزامی است)</param>
        public Region(long createBy, string name, RegionType type, long? parentId = null) : base(createBy)
        {
            InitializeRegion(name, type, parentId);
        }

        /// <summary>
        /// متد مقداردهی اولیه منطقه با استفاده از متدهای اعتبارسنجی
        /// </summary>
        /// <param name="name">نام منطقه</param>
        /// <param name="type">نوع منطقه</param>
        /// <param name="parentId">شناسه منطقه پدر</param>
        private void InitializeRegion(string name, RegionType type, long? parentId)
        {
            SetName(name);
            SetType(type);
            SetParentId(parentId);
        }

        // ===== متدهای اعتبارسنجی خصوصی =====

        /// <summary>
        /// تنظیم و اعتبارسنجی نام منطقه
        /// </summary>
        /// <param name="name">نام ورودی</param>
        /// <exception cref="DomainException">در صورت خالی بودن یا طول نامعتبر</exception>
        private void SetName(string name)
        {
            var normalized = name?.Trim();

            if (string.IsNullOrWhiteSpace(normalized))
                throw new DomainException("نام منطقه نمی‌تواند خالی باشد.", DomainConcept.Region, nameof(Name));

            if (Guard.IsInvalidLength(normalized, 2, 100))
                throw new DomainException("نام منطقه باید بین 2 تا 100 کاراکتر باشد.", DomainConcept.Region, nameof(Name));

            Name = normalized;
        }

        /// <summary>
        /// تنظیم و اعتبارسنجی نوع منطقه
        /// </summary>
        /// <param name="type">نوع منطقه</param>
        /// <exception cref="DomainException">در صورت نامعتبر بودن</exception>
        private void SetType(RegionType type)
        {
            if (Guard.IsInvalidEnum(type))
                throw new DomainException("نوع منطقه نامعتبر است.", DomainConcept.Region, nameof(Type));

            Type = type;
        }

        /// <summary>
        /// تنظیم و اعتبارسنجی شناسه منطقه پدر
        /// </summary>
        /// <param name="parentId">شناسه منطقه پدر</param>
        /// <exception cref="DomainException">در صورت نامعتبر بودن منطق سلسله‌مراتبی</exception>
        private void SetParentId(long? parentId)
        {
            // اگر نوع منطقه شهر است، ParentId الزامی است
            if (Type == RegionType.City && !parentId.HasValue)
                throw new DomainException("برای ثبت شهر، شناسه استان مادر الزامی است.", DomainConcept.Region, nameof(ParentId));

            // اگر نوع منطقه استان است، ParentId باید null باشد
            if (Type == RegionType.Province && parentId.HasValue)
                throw new DomainException("استان نمی‌تواند منطقه پدر داشته باشد.", DomainConcept.Region, nameof(ParentId));

            // جلوگیری از ارجاع به خود
            if (Guard.IsSelfReference(this.Id, parentId))
                throw new DomainException("منطقه نمی‌تواند پدر خودش باشد.", DomainConcept.Region, nameof(ParentId));

            ParentId = parentId;
        }

        // ===== متدهای به‌روزرسانی عمومی =====

        /// <summary>
        /// به‌روزرسانی نام منطقه
        /// </summary>
        /// <param name="name">نام جدید</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UpdateName(string name, long updateBy)
        {
            SetName(name);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// افزودن منطقه فرزند (مثلاً افزودن شهر به استان)
        /// </summary>
        /// <param name="child">منطقه فرزند</param>
        /// <exception cref="DomainException">در صورت خالی بودن</exception>
        public void AddChild(Region child)
        {
            if (child is null)
                throw new DomainException("منطقه فرزند نمی‌تواند خالی باشد.", DomainConcept.Region, nameof(Children));

            if (!_children.Contains(child))
            {
                _children.Add(child);
            }
        }

        /// <summary>
        /// حذف منطقه فرزند
        /// </summary>
        /// <param name="child">منطقه فرزند</param>
        public void RemoveChild(Region child)
        {
            _children.Remove(child);
        }
    }
}