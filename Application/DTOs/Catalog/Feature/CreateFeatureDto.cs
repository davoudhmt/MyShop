using Domain.Enums;

namespace Application.DTOs.Catalog.Feature
{
    /// <summary>
    /// DTO برای ساخت ویژگی جدید
    /// </summary>
    public class CreateFeatureDto
    {
        /// <summary>
        /// نام ویژگی (اجباری)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// چندگانگی ویژگی (اجباری)
        /// در جیسون می‌تونید مقدار عددی یا نام Enum رو بفرستید (مثلاً "Single" یا "Multiple")
        /// </summary>
        public FeatureMultiplicity Multiplicity { get; set; }
    }
}