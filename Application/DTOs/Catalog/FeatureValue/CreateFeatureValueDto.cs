namespace Application.DTOs.Catalog.FeatureValue
{
    /// <summary>
    /// DTO برای ساخت مقدار ویژگی جدید
    /// </summary>
    public class CreateFeatureValueDto
    {
        /// <summary>
        /// مقدار ویژگی (اجباری، بین 1 تا 100 کاراکتر)
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// شناسه ویژگی مربوطه (اجباری)
        /// </summary>
        public long FeatureId { get; set; }
    }
}