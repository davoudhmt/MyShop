using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;
using System.Collections.Generic;

namespace Domain.Entities.Catalog
{
    /// <summary>
    /// کلاس Variant (نوع قابل فروش محصول)
    /// </summary>
    public class Variant : BaseEntity
    {
        /// <summary>
        /// فیلد شناسه محصول
        /// </summary>
        public long ProductId { get; private set; }

        /// <summary>
        /// فیلد قیمت این واریانت
        /// </summary>
        public decimal Price { get; private set; }

        /// <summary>
        /// فیلد درصد تخفیف (اختیاری)
        /// </summary>
        public decimal? DiscountPercent { get; private set; }

        /// <summary>
        /// فیلد موجودی
        /// </summary>
        public int Stock { get; private set; }

        /// <summary>
        /// فیلد کد SKU یکتا (۱۰ کاراکتر)
        /// </summary>
        public SKU SKU { get; private set; } = null!;

        /// <summary>
        /// لیست داخلی مقادیر ویژگی‌های این واریانت (ترکیب)
        /// </summary>
        private readonly List<FeatureValue> _featureValues = new();

        /// <summary>
        /// مقادیر ویژگی‌های این واریانت (فقط خواندنی)
        /// </summary>
        public IReadOnlyCollection<FeatureValue> FeatureValues => _featureValues.AsReadOnly();

        /// <summary>
        /// لیست داخلی تصاویر این واریانت
        /// </summary>
        private readonly List<ProductImage> _productImages = new();

        /// <summary>
        /// تصاویر این واریانت (فقط خواندنی)
        /// </summary>
        public IReadOnlyCollection<ProductImage> ProductImages => _productImages.AsReadOnly();

        /// <summary>
        /// سازنده پیش‌فرض EF Core
        /// </summary>
        protected Variant() { }

        /// <summary>
        /// سازنده کلاس واریانت
        /// </summary>
        /// <param name="createBy">شناسه کاربر ایجادکننده</param>
        /// <param name="productId">شناسه محصول</param>
        /// <param name="price">قیمت</param>
        /// <param name="stock">موجودی</param>
        /// <param name="sku">کد SKU</param>
        /// <param name="discountPercent">درصد تخفیف (اختیاری)</param>
        public Variant(
            long createBy,
            long productId,
            decimal price,
            int stock,
            SKU sku,
            decimal? discountPercent = null) : base(createBy)
        {
            InitializeVariant(productId, price, stock, sku, discountPercent);
        }

        /// <summary>
        /// متد مقداردهی اولیه واریانت
        /// </summary>
        private void InitializeVariant(
            long productId,
            decimal price,
            int stock,
            SKU sku,
            decimal? discountPercent)
        {
            SetProductId(productId);
            SetPrice(price);
            SetStock(stock);
            SetSKU(sku);
            SetDiscountPercent(discountPercent);
        }

        /// <summary>
        /// متد اعتبارسنجی شناسه محصول
        /// </summary>
        /// <param name="productId">شناسه محصول</param>
        /// <exception cref="DomainException"></exception>
        private void SetProductId(long productId)
        {
            if (Guard.IsInvalidId(productId))
                throw new DomainException("شناسه محصول نامعتبر است.", DomainConcept.Variant, nameof(ProductId));
            ProductId = productId;
        }

        /// <summary>
        /// متد اعتبارسنجی قیمت
        /// </summary>
        /// <param name="price">قیمت</param>
        /// <exception cref="DomainException"></exception>
        private void SetPrice(decimal price)
        {
            if (Guard.IsNegative(price))
                throw new DomainException("قیمت نمی‌تواند منفی باشد.", DomainConcept.Variant, nameof(Price));
            Price = price;
        }

        /// <summary>
        /// متد اعتبارسنجی موجودی
        /// </summary>
        /// <param name="stock">موجودی</param>
        /// <exception cref="DomainException"></exception>
        private void SetStock(int stock)
        {
            if (Guard.IsNegative(stock))
                throw new DomainException("موجودی نمی‌تواند منفی باشد.", DomainConcept.Variant, nameof(Stock));
            Stock = stock;
        }

        /// <summary>
        /// متد اعتبارسنجی کد SKU
        /// </summary>
        /// <param name="sku">کد SKU</param>
        /// <exception cref="DomainException"></exception>
        private void SetSKU(SKU sku)
        {
            if (sku is null)
                throw new DomainException("کد SKU نمی‌تواند خالی باشد.", DomainConcept.Variant, nameof(SKU));
            SKU = sku;
        }

        /// <summary>
        /// متد اعتبارسنجی درصد تخفیف
        /// </summary>
        /// <param name="discountPercent">درصد تخفیف</param>
        /// <exception cref="DomainException"></exception>
        private void SetDiscountPercent(decimal? discountPercent)
        {
            if (Guard.IsOutOfRange(discountPercent, 0, 100))
                throw new DomainException("درصد تخفیف باید بین 0 تا 100 باشد.", DomainConcept.Variant, nameof(DiscountPercent));

            DiscountPercent = discountPercent;
        }

        // ===== متدهای به‌روزرسانی =====

        /// <summary>
        /// متد به‌روزرسانی قیمت
        /// </summary>
        /// <param name="price">قیمت جدید</param>
        /// <param name="updateBy">شناسه کاربر</param>
        public void UpdatePrice(decimal price, long updateBy)
        {
            SetPrice(price);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// متد به‌روزرسانی موجودی
        /// </summary>
        /// <param name="stock">موجودی جدید</param>
        /// <param name="updateBy">شناسه کاربر</param>
        public void UpdateStock(int stock, long updateBy)
        {
            SetStock(stock);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// متد به‌روزرسانی درصد تخفیف
        /// </summary>
        /// <param name="discountPercent">درصد تخفیف جدید</param>
        /// <param name="updateBy">شناسه کاربر</param>
        public void UpdateDiscountPercent(decimal? discountPercent, long updateBy)
        {
            SetDiscountPercent(discountPercent);
            UpdateModificationInfo(updateBy);
        }

        // ===== رابطه با مقادیر ویژگی (ترکیب واریانت) =====

        /// <summary>
        /// افزودن مقدار ویژگی به واریانت (با همگام‌سازی دوطرفه)
        /// </summary>
        /// <param name="featureValue">مقدار ویژگی</param>
        public void AddFeatureValue(FeatureValue featureValue)
        {
            if (featureValue is null)
                throw new DomainException("مقدار ویژگی نمی‌تواند خالی باشد.", DomainConcept.Variant, nameof(FeatureValues));

            if (!_featureValues.Contains(featureValue))
            {
                _featureValues.Add(featureValue);
                featureValue.AddVariant(this);  // sync
            }
        }

        /// <summary>
        /// حذف مقدار ویژگی از واریانت (با همگام‌سازی دوطرفه)
        /// </summary>
        /// <param name="featureValue">مقدار ویژگی</param>
        public void RemoveFeatureValue(FeatureValue featureValue)
        {
            if (_featureValues.Remove(featureValue))
                featureValue.RemoveVariant(this);  // sync
        }

        // ===== رابطه با تصاویر =====

        /// <summary>
        /// افزودن تصویر به واریانت
        /// </summary>
        /// <param name="image">تصویر</param>
        public void AddImage(ProductImage image)
        {
            if (image is null)
                throw new DomainException("تصویر نمی‌تواند خالی باشد.", DomainConcept.Variant, nameof(ProductImages));

            if (!_productImages.Contains(image))
                _productImages.Add(image);
        }

        /// <summary>
        /// حذف تصویر از واریانت
        /// </summary>
        /// <param name="image">تصویر</param>
        public void RemoveImage(ProductImage image)
        {
            _productImages.Remove(image);
        }
    }
}