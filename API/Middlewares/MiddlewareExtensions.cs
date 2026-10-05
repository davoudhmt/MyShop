namespace API.Middlewares
{
    /// <summary>
    /// متدهای توسعه‌دهنده برای ثبت آسان میدل‌ورها
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// ثبت میدل‌ور مدیریت سراسری خطاها
        /// </summary>
        /// <param name="app">نماینده برنامه</param>
        /// <returns>نماینده برنامه برای زنجیره‌سازی</returns>
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}