using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;
using System;

namespace Domain.Entities.Catalog
{
    /// <summary>
    /// کلاس FeatureValue (مقدار ممکن برای یک ویژگی)
    /// </summary>
    public class FeatureValue : BaseEntity
    {
        /// <summary>
        /// فیلد مقدار ویژگی
        /// </summary>
        public string Value { get; private set; } = string.Empty;

        /// <summary>
        /// فیلد شناسه ویژگی (این مقدار مال کدوم ویژگیه)
        /// </summary>
        public long FeatureId { get; private set; }

        // ===== رابطه با واریانت‌ها (برای همگام‌سازی دوطرفه) =====

        /// <summary>
        /// لیست داخلی واریانت‌هایی که این مقدار ویژگی را دارند
        /// </summary>
        private readonly List<Variant> _variants = new();

        /// <summary>
        /// واریانت‌هایی که این مقدار ویژگی را دارند (فقط خواندنی)
        /// </summary>
        public IReadOnlyCollection<Variant> Variants => _variants.AsReadOnly();

        /// <summary>
        /// سازنده پیش‌فرض EF Core
        /// </summary>
        protected FeatureValue() { }

        /// <summary>
        /// سازنده کلاس مقدار ویژگی
        /// </summary>
        /// <param name="createBy">شناسه کاربر ایجادکننده</param>
        /// <param name="value">مقدار ویژگی</param>
        /// <param name="featureId">شناسه ویژگی</param>
        public FeatureValue(long createBy, string value, long featureId) : base(createBy)
        {
            InitializeFeatureValue(value, featureId);
        }

        /// <summary>
        /// متد مقداردهی اولیه مقدار ویژگی
        /// </summary>
        /// <param name="value">مقدار ویژگی</param>
        /// <param name="featureId">شناسه ویژگی</param>
        private void InitializeFeatureValue(string value, long featureId)
        {
            SetValue(value);
            SetFeatureId(featureId);
        }

        /// <summary>
        /// متد اعتبارسنجی مقدار ویژگی
        /// </summary>
        /// <param name="value">مقدار ویژگی</param>
        /// <exception cref="DomainException"></exception>
        private void SetValue(string value)
        {
            var normalizedValue = value?.Trim();

            if (string.IsNullOrWhiteSpace(normalizedValue))
                throw new DomainException("مقدار ویژگی نمی‌تواند خالی باشد.", DomainConcept.FeatureValue, nameof(Value));

            if (Guard.IsInvalidLength(normalizedValue, 1, 100))
                throw new DomainException("مقدار ویژگی باید بین 1 تا 100 کاراکتر باشد.", DomainConcept.FeatureValue, nameof(Value));

            Value = normalizedValue!;
        }

        /// <summary>
        /// متد اعتبارسنجی شناسه ویژگی
        /// </summary>
        /// <param name="featureId">شناسه ویژگی</param>
        /// <exception cref="DomainException"></exception>
        private void SetFeatureId(long featureId)
        {
            if (Guard.IsInvalidId(featureId))
                throw new DomainException("شناسه ویژگی نامعتبر است.", DomainConcept.FeatureValue, nameof(FeatureId));

            FeatureId = featureId;
        }

        /// <summary>
        /// متد به‌روزرسانی مقدار ویژگی
        /// </summary>
        /// <param name="value">مقدار جدید</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UpdateValue(string value, long updateBy)
        {
            SetValue(value);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// افزودن واریانت (فقط داخلی، برای همگام‌سازی)
        /// </summary>
        /// <param name="variant">واریانت</param>
        internal void AddVariant(Variant variant)
        {
            if (!_variants.Contains(variant))
                _variants.Add(variant);
        }

        /// <summary>
        /// حذف واریانت (فقط داخلی، برای همگام‌سازی)
        /// </summary>
        /// <param name="variant">واریانت</param>
        internal void RemoveVariant(Variant variant)
        {
            _variants.Remove(variant);
        }
    }
}