namespace Application.DTOs.Identity.Auth
{
    /// <summary>
    /// نتیجه‌ی تولید توکن موقت (StepToken) برای احراز هویت دو مرحله‌ای ادمین
    /// </summary>
    public class StepTokenResultDto
    {
        /// <summary>
        /// توکن موقت (JWT) با عمر کوتاه (مثلاً ۵ دقیقه)
        /// </summary>
        public string StepToken { get; set; } = string.Empty;

        /// <summary>
        /// زمان انقضای توکن موقت
        /// </summary>
        public DateTime Expiration { get; set; }
    }
}