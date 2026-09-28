using Domain.Enums;

namespace Application.DTOs.Catalog.Feature
{
    /// <summary>
    /// DTO برای به‌روزرسانی ویژگی
    /// </summary>
    public class UpdateFeatureDto
    {
        /// <summary>
        /// شناسه ویژگی (اجباری)
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// نام ویژگی (اجباری)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// چندگانگی ویژگی (اجباری)
        /// </summary>
        public FeatureMultiplicity Multiplicity { get; set; }

        /// <summary>
        /// وضعیت ویژگی:
        /// - "Active": فعال
        /// - "Inactive": غیرفعال
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}