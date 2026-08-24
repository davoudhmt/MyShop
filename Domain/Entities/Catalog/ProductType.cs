using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities.Catalog
{
    /// <summary>
    /// کلاس ProductType (نوع محصول)
    /// </summary>
    public class ProductType : BaseEntity
    {
        /// <summary>
        /// فیلد نام نوع محصول
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// فیلد توضیحات نوع محصول
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// لیست داخلی دسته‌بندی‌هایی که این نوع محصول در آن‌هاست
        /// </summary>
        private readonly List<Category> _categories = new();

        /// <summary>
        /// دسته‌بندی‌های این نوع محصول (فقط خواندنی)
        /// </summary>
        public IReadOnlyCollection<Category> Categories => _categories.AsReadOnly();

        /// <summary>
        /// سازنده پیشفرض EF Core
        /// </summary>
        protected ProductType() { }

        /// <summary>
        /// سازنده کلاس نوع محصول
        /// </summary>
        /// <param name="createBy"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public ProductType(long createBy, string name, string? description = null) : base(createBy)
        {
            InitializeProductType(name, description);
        }

        /// <summary>
        /// متد مقداردهی نوع محصول
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        private void InitializeProductType(string name, string? description)
        {
            SetName(name);
            SetDescription(description);
        }

        /// <summary>
        /// متد اعتبارسنجی نام نوع محصول
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="DomainException"></exception>
        private void SetName(string name)
        {
            var normalizedName = name?.Trim();

            if (string.IsNullOrWhiteSpace(normalizedName))
                throw new DomainException("نام نوع محصول نمی تواند خالی باشد", DomainConcept.ProductType, nameof(Name));

            if (Guard.IsInvalidLength(normalizedName, 2, 100))
                throw new DomainException("نام نوع محصول باید بین 2 تا 100 کاراکتر باشد.", DomainConcept.ProductType, nameof(Name));

            Name = normalizedName;
        }

        /// <summary>
        /// متد اعتبارسنجی توضیحات نوع محصول
        /// </summary>
        /// <param name="description"></param>
        /// <exception cref="DomainException"></exception>
        private void SetDescription(string? description)
        {
            var normalizedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

            if (Guard.IsOverMaxLength(normalizedDescription, 500))
                throw new DomainException("توضیحات نوع محصول نمی تواند بیش از 500 کاراکتر باشد.", DomainConcept.ProductType, nameof(Description));

            Description = normalizedDescription;
        }

        /// <summary>
        /// متد بروزرسانی نام نوع محصول
        /// </summary>
        /// <param name="name"></param>
        /// <param name="updateBy"></param>
        public void UpdateProductTypeName(string name, long updateBy)
        {
            SetName(name);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// متد بروزرسانی توضیحات نوع محصول
        /// </summary>
        /// <param name="description"></param>
        /// <param name="updateBy"></param>
        public void UpdateProductTypeDescription(string? description, long updateBy)
        {
            SetDescription(description);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// افزودن دسته‌بندی (فقط داخلی، برای همگام‌سازی)
        /// </summary>
        /// <param name="category">دسته‌بندی</param>
        internal void AddCategory(Category category)
        {
            if (!_categories.Contains(category))
                _categories.Add(category);
        }

        /// <summary>
        /// حذف دسته‌بندی (فقط داخلی، برای همگام‌سازی)
        /// </summary>
        /// <param name="category">دسته‌بندی</param>
        internal void RemoveCategory(Category category)
        {
            _categories.Remove(category);
        }
    }
}
