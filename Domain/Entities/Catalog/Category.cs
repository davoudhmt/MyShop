using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities.Catalog
{
    /// <summary>
    /// کلاس Category (دسته‌بندی محصولات)
    /// </summary>
    public class Category : BaseEntity
    {
        /// <summary>
        /// فیلد نام دسته‌بندی
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// فیلد توضیحات دسته‌بندی
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// فیلد شناسه دسته‌بندی والد (برای سلسله‌مراتب)
        /// </summary>
        public long? ParentCategoryId { get; private set; }

        /// <summary>
        /// فیلد تصویر دسته‌بندی
        /// </summary>
        public ImageUrl? ImageUrl { get; private set; }

        /// <summary>
        /// فیلد کد دسته‌بندی
        /// </summary>
        public CategoryCode CategoryCode { get; private set; } = null!;

        /// <summary>
        /// سازنده پیش‌فرض EF Core
        /// </summary>
        protected Category() { }

        /// <summary>
        /// سازنده کلاس دسته‌بندی
        /// </summary>
        /// <param name="createBy">شناسه کاربر ایجادکننده</param>
        /// <param name="name">نام دسته‌بندی</param>
        /// <param name="categoryCode">کد دسته‌بندی</param>
        /// <param name="description">توضیحات دسته‌بندی (اختیاری)</param>
        /// <param name="imageUrl">تصویر دسته‌بندی (اختیاری)</param>
        /// <param name="parentCategoryId">شناسه دسته‌بندی والد (اختیاری)</param>
        public Category(
            long createBy,
            string name,
            CategoryCode categoryCode,
            string? description = null,
            ImageUrl? imageUrl = null,
            long? parentCategoryId = null) : base(createBy)
        {
            InitializeCategory(name, categoryCode, description, imageUrl, parentCategoryId);
        }

        /// <summary>
        /// متد مقداردهی اولیه دسته‌بندی
        /// این متد فقط توسط سازنده فراخوانی می‌شود
        /// </summary>
        /// <param name="name">نام دسته‌بندی</param>
        /// <param name="categoryCode">کد دسته‌بندی</param>
        /// <param name="description">توضیحات دسته‌بندی (اختیاری)</param>
        /// <param name="imageUrl">تصویر دسته‌بندی (اختیاری)</param>
        /// <param name="parentCategoryId">شناسه دسته‌بندی والد (اختیاری)</param>
        private void InitializeCategory(
            string name,
            CategoryCode categoryCode,
            string? description,
            ImageUrl? imageUrl,
            long? parentCategoryId)
        {
            SetName(name);
            SetCategoryCode(categoryCode);
            SetDescription(description);
            SetImageUrl(imageUrl);
            SetParentCategoryId(parentCategoryId);
        }

        /// <summary>
        /// متد اعتبارسنجی نام دسته‌بندی
        /// </summary>
        /// <param name="name">نام دسته‌بندی</param>
        /// <exception cref="DomainException"></exception>
        private void SetName(string name)
        {
            var normalizedName = name?.Trim();

            if (string.IsNullOrWhiteSpace(normalizedName))
                throw new DomainException("نام دسته‌بندی نمی‌تواند خالی باشد.", DomainConcept.Category, nameof(Name));

            if (Guard.IsInvalidLength(normalizedName, 2, 100))
                throw new DomainException("نام دسته‌بندی باید بین 2 تا 100 کاراکتر باشد.", DomainConcept.Category, nameof(Name));

            Name = normalizedName!;
        }

        /// <summary>
        /// متد تنظیم کد دسته‌بندی
        /// </summary>
        /// <param name="categoryCode">کد دسته‌بندی</param>
        /// <exception cref="DomainException"></exception>
        private void SetCategoryCode(CategoryCode categoryCode)
        {
            if (categoryCode is null)
                throw new DomainException("کد دسته‌بندی نمی‌تواند خالی باشد.", DomainConcept.Category, nameof(CategoryCode));

            CategoryCode = categoryCode;
        }

        /// <summary>
        /// متد اعتبارسنجی توضیحات دسته‌بندی
        /// </summary>
        /// <param name="description">توضیحات دسته‌بندی</param>
        /// <exception cref="DomainException"></exception>
        private void SetDescription(string? description)
        {
            var normalizedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

            if (Guard.IsOverMaxLength(normalizedDescription, 500))
                throw new DomainException("توضیحات دسته‌بندی نمی‌تواند بیش از 500 کاراکتر باشد.", DomainConcept.Category, nameof(Description));

            Description = normalizedDescription;
        }

        /// <summary>
        /// متد تنظیم تصویر دسته‌بندی
        /// </summary>
        /// <param name="imageUrl">تصویر دسته‌بندی</param>
        private void SetImageUrl(ImageUrl? imageUrl)
        {
            ImageUrl = imageUrl;
        }

        /// <summary>
        /// متد تنظیم شناسه دسته‌بندی والد
        /// </summary>
        /// <param name="parentCategoryId">شناسه دسته‌بندی والد</param>
        private void SetParentCategoryId(long? parentCategoryId)
        {
            if (parentCategoryId.HasValue && parentCategoryId.Value <= 0)
                throw new DomainException("شناسه دسته‌بندی والد نامعتبر است.", DomainConcept.Category, nameof(ParentCategoryId));

            if (parentCategoryId.HasValue && parentCategoryId.Value == Id)
                throw new DomainException("دسته‌بندی نمی‌تواند والد خودش باشد.", DomainConcept.Category, nameof(ParentCategoryId));

            ParentCategoryId = parentCategoryId;
        }

        /// <summary>
        /// متد به‌روزرسانی نام دسته‌بندی
        /// </summary>
        /// <param name="name">نام جدید دسته‌بندی</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UpdateCategoryName(string name, long updateBy)
        {
            SetName(name);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// متد به‌روزرسانی توضیحات دسته‌بندی
        /// </summary>
        /// <param name="description">توضیحات جدید دسته‌بندی (اختیاری)</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UpdateCategoryDescription(string? description, long updateBy)
        {
            SetDescription(description);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// متد به‌روزرسانی تصویر دسته‌بندی
        /// </summary>
        /// <param name="imageUrl">تصویر جدید دسته‌بندی (اختیاری)</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UpdateCategoryImageUrl(ImageUrl? imageUrl, long updateBy)
        {
            SetImageUrl(imageUrl);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// متد تغییر دسته‌بندی والد
        /// </summary>
        /// <param name="parentCategoryId">شناسه دسته‌بندی والد جدید (اختیاری)</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void ChangeParentCategory(long? parentCategoryId, long updateBy)
        {
            SetParentCategoryId(parentCategoryId);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// متد تبدیل به زیردسته (تبدیل به ریشه)
        /// </summary>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void MakeRootCategory(long updateBy)
        {
            SetParentCategoryId(null);
            UpdateModificationInfo(updateBy);
        }
    }
}
