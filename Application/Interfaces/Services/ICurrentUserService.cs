namespace Application.Interfaces.Services
{
    /// <summary>
    /// سرویس دسترسی به اطلاعات کاربر فعلی (احراز هویت شده)
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// شناسه عددی کاربر فعلی
        /// </summary>
        long UserId { get; }

        /// <summary>
        /// آیا کاربر در حال حاضر احراز هویت شده است؟
        /// </summary>
        bool IsAuthenticated { get; }
    }
}