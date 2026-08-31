using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities.Catalog
{
    /// <summary>
    /// کلاس ProductImage (تصویر محصول یا واریانت)
    /// </summary>
    public class ProductImage : BaseEntity
    {
        /// <summary>
        /// فیلد آدرس تصویر
        /// </summary>
        public ImageUrl ImageUrl { get; private set; } = null!;

        /// <summary>
        /// فیلد شناسه محصول (اجباری)
        /// </summary>
        public long ProductId { get; private set; }

        /// <summary>
        /// فیلد شناسه واریانت (اختیاری - اگه خالی باشه یعنی گالری اصلی محصول)
        /// </summary>
        public long? VariantId { get; private set; }

        /// <summary>
        /// فیلد نشانه تصویر اصلی (کاور برای نمایش در لیست‌ها)
        /// </summary>
        public bool IsMain { get; private set; }

        /// <summary>
        /// فیلد متن جایگزین تصویر (برای سئو و دسترسی‌پذیری)
        /// </summary>
        public string? AltText { get; private set; }

        /// <summary>
        /// سازنده پیش‌فرض EF Core
        /// </summary>
        protected ProductImage() { }

        /// <summary>
        /// سازنده کلاس تصویر محصول
        /// </summary>
        /// <param name="createBy">شناسه کاربر ایجادکننده</param>
        /// <param name="imageUrl">آدرس تصویر</param>
        /// <param name="productId">شناسه محصول</param>
        /// <param name="variantId">شناسه واریانت (اختیاری)</param>
        /// <param name="isMain">آیا این تصویر اصلی است؟ (پیش‌فرض: خیر)</param>
        /// <param name="altText">متن جایگزین تصویر (اختیاری)</param>
        public ProductImage(
            long createBy,
            ImageUrl imageUrl,
            long productId,
            long? variantId = null,
            bool isMain = false,
            string? altText = null) : base(createBy)
        {
            InitializeProductImage(imageUrl, productId, variantId, isMain, altText);
        }

        /// <summary>
        /// متد مقداردهی اولیه تصویر محصول
        /// </summary>
        private void InitializeProductImage(
            ImageUrl imageUrl,
            long productId,
            long? variantId,
            bool isMain,
            string? altText)
        {
            SetImageUrl(imageUrl);
            SetProductId(productId);
            SetVariantId(variantId);
            IsMain = isMain;
            SetAltText(altText);
        }

        /// <summary>
        /// متد اعتبارسنجی آدرس تصویر
        /// </summary>
        /// <param name="imageUrl">آدرس تصویر</param>
        /// <exception cref="DomainException"></exception>
        private void SetImageUrl(ImageUrl imageUrl)
        {
            if (imageUrl is null)
                throw new DomainException("تصویر نمی‌تواند خالی باشد.", DomainConcept.ProductImage, nameof(ImageUrl));
            ImageUrl = imageUrl;
        }

        /// <summary>
        /// متد اعتبارسنجی شناسه محصول
        /// </summary>
        /// <param name="productId">شناسه محصول</param>
        /// <exception cref="DomainException"></exception>
        private void SetProductId(long productId)
        {
            if (Guard.IsInvalidId(productId))
                throw new DomainException("شناسه محصول نامعتبر است.", DomainConcept.ProductImage, nameof(ProductId));
            ProductId = productId;
        }

        /// <summary>
        /// متد اعتبارسنجی شناسه واریانت
        /// </summary>
        /// <param name="variantId">شناسه واریانت (اختیاری)</param>
        /// <exception cref="DomainException"></exception>
        private void SetVariantId(long? variantId)
        {
            if (variantId.HasValue && Guard.IsInvalidId(variantId.Value))
                throw new DomainException("شناسه واریانت نامعتبر است.", DomainConcept.ProductImage, nameof(VariantId));
            VariantId = variantId;
        }

        /// <summary>
        /// متد اعتبارسنجی متن جایگزین
        /// </summary>
        /// <param name="altText">متن جایگزین</param>
        /// <exception cref="DomainException"></exception>
        private void SetAltText(string? altText)
        {
            var normalizedAltText = string.IsNullOrWhiteSpace(altText) ? null : altText.Trim();

            if (Guard.IsOverMaxLength(normalizedAltText, 250))
                throw new DomainException("متن جایگزین نمی‌تواند بیش از 250 کاراکتر باشد.", DomainConcept.ProductImage, nameof(AltText));

            AltText = normalizedAltText;
        }

        /// <summary>
        /// متد به‌روزرسانی آدرس تصویر
        /// </summary>
        /// <param name="imageUrl">آدرس تصویر جدید</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UpdateImageUrl(ImageUrl imageUrl, long updateBy)
        {
            SetImageUrl(imageUrl);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// متد تبدیل این تصویر به تصویر اصلی (کاور)
        /// </summary>
        public void MarkAsMain()
        {
            IsMain = true;
        }

        /// <summary>
        /// متد حذف حالت اصلی از این تصویر
        /// </summary>
        public void UnmarkAsMain()
        {
            IsMain = false;
        }

        /// <summary>
        /// متد به‌روزرسانی متن جایگزین
        /// </summary>
        /// <param name="altText">متن جایگزین جدید</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UpdateAltText(string? altText, long updateBy)
        {
            SetAltText(altText);
            UpdateModificationInfo(updateBy);
        }
    }
}