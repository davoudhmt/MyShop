using Domain.Common;
using Domain.Constants;
using Domain.Enums;
using Domain.Exceptions;
using System.Collections.Generic;

namespace Domain.Entities.Catalog
{
    /// <summary>
    /// کلاس Tag (برچسب مشترک بین محصولات)
    /// </summary>
    public class Tag : BaseEntity
    {
        /// <summary>
        /// فیلد نام برچسب
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// لیست داخلی محصولاتی که این برچسب را دارند
        /// </summary>
        private readonly List<Product> _products = new();

        /// <summary>
        /// محصولاتی که این برچسب را دارند (فقط خواندنی)
        /// </summary>
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        /// <summary>
        /// سازنده پیش‌فرض EF Core
        /// </summary>
        protected Tag() { }

        /// <summary>
        /// سازنده کلاس برچسب
        /// </summary>
        public Tag(long createBy, string name) : base(createBy)
        {
            SetName(name);
        }

        /// <summary>
        /// متد اعتبارسنجی نام برچسب
        /// </summary>
        private void SetName(string name)
        {
            var normalizedName = name?.Trim();

            if (string.IsNullOrWhiteSpace(normalizedName))
                throw new DomainException("نام برچسب نمی‌تواند خالی باشد.", DomainConcept.Tag, nameof(Name));

            if (Guard.IsInvalidLength(normalizedName, 2, 50))
                throw new DomainException("نام برچسب باید بین 2 تا 50 کاراکتر باشد.", DomainConcept.Tag, nameof(Name));

            Name = normalizedName!;
        }

        /// <summary>
        /// متد به‌روزرسانی نام برچسب
        /// </summary>
        public void UpdateName(string name, long updateBy)
        {
            SetName(name);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// حذف منطقی برچسب
        /// قانون: برچسبی که در محصولات فعال استفاده شده، قابل حذف نیست
        /// </summary>
        public override void Delete(long updateBy)
        {
            var hasActiveProducts = _products.Any(p => !p.IsDeleted);

            if (hasActiveProducts)
                throw new DomainException(
                    "این برچسب در محصولات فعال استفاده شده است. ابتدا برچسب را از محصولات حذف کنید.",
                    DomainConcept.Tag,
                    nameof(Products));

            base.Delete(updateBy);
        }

        // ===== همگام‌سازی دوطرفه با Product =====

        internal void AddProduct(Product product)
        {
            if (!_products.Contains(product))
                _products.Add(product);
        }

        internal void RemoveProduct(Product product)
        {
            _products.Remove(product);
        }
    }
}