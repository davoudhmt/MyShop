using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities.Catalog
{
    /// <summary>
    /// کلاس Product (محصول اصلی)
    /// </summary>
    public class Product : BaseEntity
    {
        /// <summary>
        /// فیلد نام محصول
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// فیلد توضیحات محصول
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// فیلد شناسه دسته‌بندی
        /// </summary>
        public long CategoryId { get; private set; }

        /// <summary>
        /// فیلد شناسه برند (اختیاری)
        /// </summary>
        public long? BrandId { get; private set; }

        /// <summary>
        /// فیلد شناسه نوع محصول
        /// </summary>
        public long ProductTypeId { get; private set; }

        /// <summary>
        /// فیلد کد محصول (۶ کاراکتر)
        /// </summary>
        public ProductCode ProductCode { get; private set; } = null!;

        /// <summary>
        /// لیست داخلی واریانت‌های محصول
        /// </summary>
        private readonly List<Variant> _variants = new();

        /// <summary>
        /// واریانت‌های محصول (فقط خواندنی)
        /// </summary>
        public IReadOnlyCollection<Variant> Variants => _variants.AsReadOnly();

        /// <summary>
        /// لیست داخلی تصاویر گالری اصلی محصول
        /// </summary>
        private readonly List<ProductImage> _productImages = new();

        /// <summary>
        /// تصاویر گالری اصلی محصول (فقط خواندنی)
        /// </summary>
        public IReadOnlyCollection<ProductImage> ProductImages => _productImages.AsReadOnly();

        /// <summary>
        /// لیست داخلی مقادیر ویژگی‌های یگانه (توضیحی)
        /// </summary>
        private readonly List<FeatureValue> _featureValues = new();

        /// <summary>
        /// مقادیر ویژگی‌های یگانه محصول (فقط خواندنی)
        /// </summary>
        public IReadOnlyCollection<FeatureValue> FeatureValues => _featureValues.AsReadOnly();

        /// <summary>
        /// فیلد توضیحات مختصر
        /// </summary>
        public string ShortDescription { get; private set; } = string.Empty;

        /// <summary>
        /// فیلد آدرس URL (اسلاگ)
        /// </summary>
        public Slug Slug { get; private set; } = null!;

        /// <summary>
        /// فیلد تنظیمات سئو
        /// </summary>
        public Seo? Seo { get; private set; }

        /// <summary>
        /// لیست داخلی برچسب‌های محصول
        /// </summary>
        private readonly List<Tag> _tags = new();

        /// <summary>
        /// برچسب‌های محصول (فقط خواندنی)
        /// </summary>
        public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

        /// <summary>
        /// سازنده پیش‌فرض EF Core
        /// </summary>
        protected Product() { }

        /// <summary>
        /// سازنده کلاس محصول
        /// </summary>
        public Product(
            long createBy,
            string name,
            string shortDescription,
            string description,
            long categoryId,
            long productTypeId,
            ProductCode productCode,
            Slug slug,
            Seo? seo = null,
            long? brandId = null) : base(createBy)
        {
            InitializeProduct(name, shortDescription, description, categoryId, productTypeId, productCode, slug, seo, brandId);
        }

        /// <summary>
        /// متد مقداردهی اولیه محصول
        /// </summary>
        private void InitializeProduct(
            string name,
            string shortDescription,
            string description,
            long categoryId,
            long productTypeId,
            ProductCode productCode,
            Slug slug,
            Seo? seo,
            long? brandId)
        {
            SetName(name);
            SetShortDescription(shortDescription);
            SetDescription(description);
            SetCategoryId(categoryId);
            SetProductTypeId(productTypeId);
            SetProductCode(productCode);
            SetSlug(slug);
            SetSeo(seo);
            SetBrandId(brandId);
        }

        /// <summary>
        /// متد اعتبارسنجی نام محصول
        /// </summary>
        private void SetName(string name)
        {
            var normalizedName = name?.Trim();

            if (string.IsNullOrWhiteSpace(normalizedName))
                throw new DomainException("نام محصول نمی‌تواند خالی باشد.", DomainConcept.Product, nameof(Name));

            if (Guard.IsInvalidLength(normalizedName, 2, 200))
                throw new DomainException("نام محصول باید بین 2 تا 200 کاراکتر باشد.", DomainConcept.Product, nameof(Name));

            Name = normalizedName!;
        }

        /// <summary>
        /// متد اعتبارسنجی توضیحات مختصر
        /// </summary>
        private void SetShortDescription(string shortDescription)
        {
            var normalized = shortDescription?.Trim();

            if (string.IsNullOrWhiteSpace(normalized))
                throw new DomainException("توضیحات مختصر نمی‌تواند خالی باشد.", DomainConcept.Product, nameof(ShortDescription));

            if (Guard.IsOverMaxLength(normalized, 500))
                throw new DomainException("توضیحات مختصر نمی‌تواند بیش از 500 کاراکتر باشد.", DomainConcept.Product, nameof(ShortDescription));

            ShortDescription = normalized!;
        }

        /// <summary>
        /// متد اعتبارسنجی توضیحات محصول
        /// </summary>
        private void SetDescription(string description)
        {
            var normalizedDescription = description?.Trim() ?? string.Empty;

            if (Guard.IsOverMaxLength(normalizedDescription, 2000))
                throw new DomainException("توضیحات محصول نمی‌تواند بیش از 2000 کاراکتر باشد.", DomainConcept.Product, nameof(Description));

            Description = normalizedDescription;
        }

        /// <summary>
        /// متد اعتبارسنجی شناسه دسته‌بندی
        /// </summary>
        private void SetCategoryId(long categoryId)
        {
            if (Guard.IsInvalidId(categoryId))
                throw new DomainException("شناسه دسته‌بندی نامعتبر است.", DomainConcept.Product, nameof(CategoryId));
            CategoryId = categoryId;
        }

        /// <summary>
        /// متد اعتبارسنجی شناسه نوع محصول
        /// </summary>
        private void SetProductTypeId(long productTypeId)
        {
            if (Guard.IsInvalidId(productTypeId))
                throw new DomainException("شناسه نوع محصول نامعتبر است.", DomainConcept.Product, nameof(ProductTypeId));
            ProductTypeId = productTypeId;
        }

        /// <summary>
        /// متد اعتبارسنجی کد محصول
        /// </summary>
        private void SetProductCode(ProductCode productCode)
        {
            if (productCode is null)
                throw new DomainException("کد محصول نمی‌تواند خالی باشد.", DomainConcept.Product, nameof(ProductCode));
            ProductCode = productCode;
        }

        /// <summary>
        /// متد اعتبارسنجی شناسه برند (اختیاری)
        /// </summary>
        private void SetBrandId(long? brandId)
        {
            if (brandId.HasValue && Guard.IsInvalidId(brandId.Value))
                throw new DomainException("شناسه برند نامعتبر است.", DomainConcept.Product, nameof(BrandId));
            BrandId = brandId;
        }

        /// <summary>
        /// متد اعتبارسنجی اسلاگ Url
        /// </summary>
        /// <param name="slug"></param>
        /// <exception cref="DomainException"></exception>
        private void SetSlug(Slug slug)
        {
            if (slug is null)
                throw new DomainException("اسلاگ نمی‌تواند خالی باشد.", DomainConcept.Product, nameof(Slug));
            Slug = slug;
        }

        /// <summary>
        /// متد مقداردهی سئو
        /// </summary>
        /// <param name="seo"></param>
        private void SetSeo(Seo? seo)
        {
            Seo = seo;
        }

        // ===== متدهای به‌روزرسانی =====

        /// <summary>
        /// متد به‌روزرسانی نام محصول
        /// </summary>
        public void UpdateName(string name, long updateBy)
        {
            SetName(name);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// متد بروزرسانی توضیحات مختصر محصول
        /// </summary>
        /// <param name="shortDescription"></param>
        /// <param name="updateBy"></param>
        public void UpdateShortDescription(string shortDescription, long updateBy)
        {
            SetShortDescription(shortDescription);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// متد به‌روزرسانی توضیحات محصول
        /// </summary>
        public void UpdateDescription(string description, long updateBy)
        {
            SetDescription(description);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// متد به‌روزرسانی دسته‌بندی
        /// </summary>
        public void UpdateCategory(long categoryId, long updateBy)
        {
            SetCategoryId(categoryId);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// متد به‌روزرسانی نوع محصول
        /// </summary>
        public void UpdateProductType(long productTypeId, long updateBy)
        {
            SetProductTypeId(productTypeId);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// متد به‌روزرسانی برند
        /// </summary>
        public void UpdateBrand(long? brandId, long updateBy)
        {
            SetBrandId(brandId);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// متد بروزرسانی اسلاگ Url
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="updateBy"></param>
        public void UpdateSlug(Slug slug, long updateBy)
        {
            SetSlug(slug);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// متد بروزرسانی سئو
        /// </summary>
        /// <param name="seo"></param>
        /// <param name="updateBy"></param>
        public void UpdateSeo(Seo? seo, long updateBy)
        {
            SetSeo(seo);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// حذف منطقی محصول و تمام وابستگی‌های مستقیم آن
        /// </summary>
        public override void Delete(long updateBy)
        {
            // ۱. حذف منطقی تمام واریانت‌های فعال این محصول
            foreach (var variant in _variants.Where(v => !v.IsDeleted).ToList())
            {
                variant.Delete(updateBy);
            }

            // ۲. حذف منطقی تمام تصاویر فعال این محصول
            foreach (var image in _productImages.Where(i => !i.IsDeleted).ToList())
            {
                image.Delete(updateBy);
            }

            // ۳. در نهایت حذف خود محصول
            base.Delete(updateBy);
        }

        // ===== مدیریت واریانت‌ها =====

        /// <summary>
        /// افزودن واریانت به محصول
        /// </summary>
        public void AddVariant(Variant variant)
        {
            if (variant is null)
                throw new DomainException("واریانت نمی‌تواند خالی باشد.", DomainConcept.Product, nameof(Variants));

            if (!_variants.Contains(variant))
                _variants.Add(variant);
        }

        /// <summary>
        /// حذف واریانت از محصول
        /// </summary>
        public void RemoveVariant(Variant variant)
        {
            _variants.Remove(variant);
        }

        // ===== مدیریت تصاویر گالری اصلی =====

        /// <summary>
        /// افزودن تصویر به گالری اصلی محصول
        /// </summary>
        public void AddImage(ProductImage image)
        {
            if (image is null)
                throw new DomainException("تصویر نمی‌تواند خالی باشد.", DomainConcept.Product, nameof(ProductImages));

            if (!_productImages.Contains(image))
                _productImages.Add(image);
        }

        /// <summary>
        /// حذف تصویر از گالری اصلی محصول
        /// </summary>
        public void RemoveImage(ProductImage image)
        {
            _productImages.Remove(image);
        }

        /// <summary>
        /// تنظیم تصویر اصلی (کاور) - فقط یک تصویر می‌تونه main باشه
        /// </summary>
        public void SetMainImage(long imageId, long updateBy)
        {
            foreach (var img in _productImages)
            {
                if (img.Id == imageId)
                    img.MarkAsMain();
                else
                    img.UnmarkAsMain();
            }
            UpdateModificationInfo(updateBy);
        }

        // ===== مدیریت ویژگی‌های یگانه =====

        /// <summary>
        /// افزودن مقدار ویژگی یگانه به محصول
        /// </summary>
        public void AddFeatureValue(FeatureValue featureValue)
        {
            if (featureValue is null)
                throw new DomainException("مقدار ویژگی نمی‌تواند خالی باشد.", DomainConcept.Product, nameof(FeatureValues));

            if (!_featureValues.Contains(featureValue))
                _featureValues.Add(featureValue);
        }

        /// <summary>
        /// حذف مقدار ویژگی یگانه از محصول
        /// </summary>
        public void RemoveFeatureValue(FeatureValue featureValue)
        {
            _featureValues.Remove(featureValue);
        }

        /// <summary>
        /// افزودن برچسب به محصول
        /// </summary>
        /// <param name="tag"></param>
        /// <exception cref="DomainException"></exception>
        public void AddTag(Tag tag)
        {
            if (tag is null)
                throw new DomainException("برچسب نمی‌تواند خالی باشد.", DomainConcept.Product, nameof(Tags));

            if (!_tags.Contains(tag))
            {
                _tags.Add(tag);
                tag.AddProduct(this);
            }
        }

        /// <summary>
        /// حذف برچسب از محصول
        /// </summary>
        /// <param name="tag"></param>
        public void RemoveTag(Tag tag)
        {
            if (_tags.Remove(tag))
                tag.RemoveProduct(this);
        }
    }
}