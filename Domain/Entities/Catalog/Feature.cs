using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities.Catalog
{
    /// <summary>
    /// کلاس Feature (ویژگی/مشخصه محصول)
    /// </summary>
    public class Feature : BaseEntity
    {
        /// <summary>
        /// فیلد نام ویژگی
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// فیلد چندگانگی ویژگی (یگانه یا چندگانه)
        /// </summary>
        public FeatureMultiplicity Multiplicity { get; private set; }

        /// <summary>
        /// لیست داخلی انواع محصولاتی که این ویژگی را دارند
        /// </summary>
        private readonly List<ProductType> _productTypes = new();

        /// <summary>
        /// انواع محصولاتی که این ویژگی را دارند (فقط خواندنی)
        /// </summary>
        public IReadOnlyCollection<ProductType> ProductTypes => _productTypes.AsReadOnly();

        /// <summary>
        /// لیست داخلی مقادیر این ویژگی
        /// </summary>
        private readonly List<FeatureValue> _values = new();

        /// <summary>
        /// مقادیر این ویژگی (فقط خواندنی)
        /// </summary>
        public IReadOnlyCollection<FeatureValue> Values => _values.AsReadOnly();

        /// <summary>
        /// سازنده پیش‌فرض EF Core
        /// </summary>
        protected Feature() { }

        /// <summary>
        /// سازنده کلاس ویژگی
        /// </summary>
        /// <param name="createBy">شناسه کاربر ایجادکننده</param>
        /// <param name="name">نام ویژگی</param>
        /// <param name="multiplicity">نوع چندگانگی</param>
        public Feature(long createBy, string name, FeatureMultiplicity multiplicity) : base(createBy)
        {
            InitializeFeature(name, multiplicity);
        }

        /// <summary>
        /// متد مقداردهی اولیه ویژگی
        /// این متد فقط توسط سازنده فراخوانی می‌شود
        /// </summary>
        /// <param name="name">نام ویژگی</param>
        /// <param name="multiplicity">نوع چندگانگی</param>
        private void InitializeFeature(string name, FeatureMultiplicity multiplicity)
        {
            SetName(name);
            SetMultiplicity(multiplicity);
        }

        /// <summary>
        /// متد اعتبارسنجی نام ویژگی
        /// </summary>
        /// <param name="name">نام ویژگی</param>
        /// <exception cref="DomainException"></exception>
        private void SetName(string name)
        {
            var normalizedName = name?.Trim();

            if (string.IsNullOrWhiteSpace(normalizedName))
                throw new DomainException("نام ویژگی نمی‌تواند خالی باشد.", DomainConcept.Feature, nameof(Name));

            if (Guard.IsInvalidLength(normalizedName, 2, 100))
                throw new DomainException("نام ویژگی باید بین 2 تا 100 کاراکتر باشد.", DomainConcept.Feature, nameof(Name));

            Name = normalizedName!;
        }

        /// <summary>
        /// متد تنظیم چندگانگی ویژگی
        /// </summary>
        /// <param name="multiplicity">نوع چندگانگی</param>
        /// <exception cref="DomainException"></exception>
        private void SetMultiplicity(FeatureMultiplicity multiplicity)
        {
            if (Guard.IsInvalidEnum(multiplicity))
                throw new DomainException("نوع چندگانگی نامعتبر است.", DomainConcept.Feature, nameof(Multiplicity));

            Multiplicity = multiplicity;
        }

        /// <summary>
        /// متد به‌روزرسانی نام ویژگی
        /// </summary>
        /// <param name="name">نام جدید ویژگی</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UpdateFeatureName(string name, long updateBy)
        {
            SetName(name);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// متد به‌روزرسانی چندگانگی ویژگی
        /// </summary>
        /// <param name="multiplicity">نوع چندگانگی جدید</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UpdateFeatureMultiplicity(FeatureMultiplicity multiplicity, long updateBy)
        {
            SetMultiplicity(multiplicity);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// افزودن نوع محصول به ویژگی (با همگام‌سازی دوطرفه)
        /// </summary>
        /// <param name="productType">نوع محصول</param>
        public void AddProductType(ProductType productType)
        {
            if (productType is null)
                throw new DomainException("نوع محصول نمی‌تواند خالی باشد.", DomainConcept.Feature, nameof(ProductTypes));

            if (!_productTypes.Contains(productType))
            {
                _productTypes.Add(productType);
                productType.AddFeature(this);  // همگام‌سازی سمت دیگه
            }
        }

        /// <summary>
        /// حذف نوع محصول از ویژگی (با همگام‌سازی دوطرفه)
        /// </summary>
        /// <param name="productType">نوع محصول</param>
        public void RemoveProductType(ProductType productType)
        {
            if (_productTypes.Remove(productType))
                productType.RemoveFeature(this);  // همگام‌سازی سمت دیگه
        }
    }
}