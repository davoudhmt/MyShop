namespace Domain.Constants;

/// <summary>
/// مفاهیم استاندارد دامنه برای استفاده در استثناها
/// استفاده از const string به جای Enum برای حفظ مقیاس‌پذیری و جلوگیری از God Enum
/// </summary>
public static class DomainConcept
{
    #region Catalog

    /// <summary>
    /// مفهوم برند در ماژول کاتالوگ
    /// </summary>
    public const string Brand = "Catalog.Brand";

    /// <summary>
    /// مفهوم دسته‌بندی در ماژول کاتالوگ
    /// </summary>
    public const string Category = "Catalog.Category";

    /// <summary>
    /// مفهوم محصول در ماژول کاتالوگ
    /// </summary>
    public const string Product = "Catalog.Product";

    /// <summary>
    /// مفهوم کد محصول (Value Object) در ماژول کاتالوگ
    /// </summary>
    public const string ProductCode = "Catalog.ProductCode";

    /// <summary>
    /// مفهوم اسلاگ یا آدرس URL (Value Object) در ماژول کاتالوگ
    /// </summary>
    public const string Slug = "Catalog.Slug";

    /// <summary>
    /// مفهوم تنظیمات سئو (Value Object) در ماژول کاتالوگ
    /// </summary>
    public const string Seo = "Catalog.Seo";

    /// <summary>
    /// مفهوم کد یکتای واریانت یا SKU (Value Object) در ماژول کاتالوگ
    /// </summary>
    public const string SKU = "Catalog.SKU";

    /// <summary>
    /// مفهوم آدرس تصویر (Value Object) در ماژول کاتالوگ
    /// </summary>
    public const string ImageUrl = "Catalog.ImageUrl";

    /// <summary>
    /// مفهوم کد دسته‌بندی (Value Object) در ماژول کاتالوگ
    /// </summary>
    public const string CategoryCode = "Catalog.CategoryCode";

    /// <summary>
    /// مفهوم ویژگی محصول در ماژول کاتالوگ
    /// </summary>
    public const string Feature = "Catalog.Feature";

    /// <summary>
    /// مفهوم مقدار ویژگی در ماژول کاتالوگ
    /// </summary>
    public const string FeatureValue = "Catalog.FeatureValue";

    /// <summary>
    /// مفهوم نوع محصول در ماژول کاتالوگ
    /// </summary>
    public const string ProductType = "Catalog.ProductType";

    /// <summary>
    /// مفهوم برچسب در ماژول کاتالوگ
    /// </summary>
    public const string Tag = "Catalog.Tag";

    /// <summary>
    /// مفهوم واریانت یا نوع قابل فروش محصول در ماژول کاتالوگ
    /// </summary>
    public const string Variant = "Catalog.Variant";

    /// <summary>
    /// مفهوم تصویر محصول یا واریانت در ماژول کاتالوگ
    /// </summary>
    public const string ProductImage = "Catalog.ProductImage";

    #endregion

    #region Identity

    /// <summary>
    /// مفهوم شخص در ماژول هویت
    /// </summary>
    public const string Person = "Identity.Person";

    /// <summary>
    /// مفهوم کاربر در ماژول هویت
    /// </summary>
    public const string User = "Identity.User";

    /// <summary>
    /// مفهوم آدرس در ماژول هویت
    /// </summary>
    public const string Address = "Identity.Address";

    /// <summary>
    /// مفهوم استان، شهر و... در ماژول هویت
    /// </summary>
    public const string Region = "Identity.Region";

    /// <summary>
    /// مفهوم مجوز دسترسی در ماژول هویت
    /// </summary>
    public const string Permission = "Identity.Permission";

    /// <summary>
    /// مفهوم نقش در ماژول هویت
    /// </summary>
    public const string Role = "Identity.Role";

    public const string UserRole = "Identity.UserRole";

    public const string RolePermission = "Identity.RolePermission";

    #endregion

    #region Ordering
    #endregion
}