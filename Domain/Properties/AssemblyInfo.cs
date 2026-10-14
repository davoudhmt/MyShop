// =============================================================================
// Assembly-Level Configuration for Domain Layer
// =============================================================================
// این فایل تنظیمات سطح Assembly لایه Domain را تعریف می‌کند.
// 
// InternalsVisibleTo:
// این Attribute به Assemblyهای مشخص‌شده اجازه می‌دهد تا به اعضای 
// internal (غیر عمومی) این Assembly دسترسی داشته باشند.
//
// دلیل استفاده:
// در الگوی DDD، برخی متدها (مثل AddChild در Category یا AddFeature در ProductType)
// باید فقط توسط لایه‌های Infra (برای EF Core) و Application (برای همگام‌سازی)
// قابل دسترسی باشند، اما نباید در API عمومی موجودیت در دسترس باشند.
// این روش، Encapsulation را حفظ می‌کند بدون اینکه نیاز به public کردن متدها باشد.
// =============================================================================

using System.Runtime.CompilerServices;

/// <summary>
/// مجوز دسترسی به اعضای internal لایه Domain برای لایه Infrastructure
/// (مورد نیاز برای EF Core جهت بارگذاری روابط و همگام‌سازی)
/// </summary>
[assembly: InternalsVisibleTo("Infra")]

/// <summary>
/// مجوز دسترسی به اعضای internal لایه Domain برای لایه Application
/// (مورد نیاز برای سرویس‌ها جهت مدیریت روابط بین موجودیت‌ها)
/// </summary>
[assembly: InternalsVisibleTo("Application")]