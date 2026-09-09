using Domain.Entities.Catalog;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infra.Persistence.Seed
{
    /// <summary>
    /// کلاس داده‌های اولیه ماژول کاتالوگ
    /// </summary>
    public class CatalogDbSeeder : ICatalogSeeder
    {
        private readonly MyShopDbContext _db;
        private const long SystemUser = 1;

        public CatalogDbSeeder(MyShopDbContext db)
        {
            _db = db;
        }

        public async Task SeedAsync()
        {
            if (await _db.Categories.AnyAsync())
                return;

            // ===== مرحله ۱: موجودیت‌های پایه =====
            var categories = await SeedCategoriesAsync();
            var brands = await SeedBrandsAsync();
            var productTypes = await SeedProductTypesAsync(categories);
            var features = await SeedFeaturesAsync();
            var tags = await SeedTagsAsync();

            // SaveChanges برای گرفتن Id (چون بعضی سازنده‌ها Id می‌خوان)
            await _db.SaveChangesAsync();

            // ===== مرحله ۲: وابسته‌ها =====
            await SeedFeatureValuesAsync(features);
            await SeedProductsAsync(categories, brands, productTypes, tags);

            // SaveChanges دوم برای گرفتن Id محصولات
            await _db.SaveChangesAsync();

            await SeedVariantsAsync();

            await _db.SaveChangesAsync();
        }

        // ===== دسته‌بندی‌ها =====
        private async Task<(Category Mobile, Category Laptop)> SeedCategoriesAsync()
        {
            var mobile = new Category(
                createBy: SystemUser,
                name: "موبایل",
                categoryCode: new CategoryCode("CAT-MOB"),
                description: "انواع گوشی موبایل");
            mobile.Activate(SystemUser);

            var laptop = new Category(
                createBy: SystemUser,
                name: "لپ‌تاپ",
                categoryCode: new CategoryCode("CAT-LAP"),
                description: "انواع لپ‌تاپ");
            laptop.Activate(SystemUser);

            await _db.Categories.AddRangeAsync(mobile, laptop);
            return (mobile, laptop);
        }

        // ===== برندها =====
        private async Task<(Brand Samsung, Brand Apple)> SeedBrandsAsync()
        {
            var samsung = new Brand(
                createBy: SystemUser,
                name: "سامسونگ",
                description: "برند کره‌ای سامسونگ");
            samsung.Activate(SystemUser);

            var apple = new Brand(
                createBy: SystemUser,
                name: "اپل",
                description: "برند آمریکایی اپل");
            apple.Activate(SystemUser);

            await _db.Brands.AddRangeAsync(samsung, apple);
            return (samsung, apple);
        }

        // ===== انواع محصول =====
        private async Task<(ProductType SmartPhone, ProductType UltraBook)> SeedProductTypesAsync(
            (Category Mobile, Category Laptop) categories)
        {
            var smartPhone = new ProductType(
                createBy: SystemUser,
                name: "گوشی هوشمند",
                description: "گوشی‌های هوشمند لمسی");
            smartPhone.Activate(SystemUser);
            smartPhone.AddCategory(categories.Mobile);

            var ultraBook = new ProductType(
                createBy: SystemUser,
                name: "اولترابوک",
                description: "لپ‌تاپ‌های سبک و باریک");
            ultraBook.Activate(SystemUser);
            ultraBook.AddCategory(categories.Laptop);

            await _db.ProductTypes.AddRangeAsync(smartPhone, ultraBook);
            return (smartPhone, ultraBook);
        }

        // ===== ویژگی‌ها =====
        private async Task<(Feature ScreenSize, Feature RAM)> SeedFeaturesAsync()
        {
            var screenSize = new Feature(
                createBy: SystemUser,
                name: "اندازه صفحه نمایش",
                multiplicity: FeatureMultiplicity.Single);
            screenSize.Activate(SystemUser);

            var ram = new Feature(
                createBy: SystemUser,
                name: "مقدار رم",
                multiplicity: FeatureMultiplicity.Single);
            ram.Activate(SystemUser);

            await _db.Features.AddRangeAsync(screenSize, ram);
            return (screenSize, ram);
        }

        // ===== مقادیر ویژگی =====
        private async Task SeedFeatureValuesAsync((Feature ScreenSize, Feature RAM) features)
        {
            // مقادیر برای اندازه صفحه نمایش
            var size61 = new FeatureValue(SystemUser, "6.1 اینچ", features.ScreenSize.Id);
            var size67 = new FeatureValue(SystemUser, "6.7 اینچ", features.ScreenSize.Id);

            // مقادیر برای رم
            var ram8 = new FeatureValue(SystemUser, "8 گیگابایت", features.RAM.Id);
            var ram12 = new FeatureValue(SystemUser, "12 گیگابایت", features.RAM.Id);

            await _db.FeatureValues.AddRangeAsync(size61, size67, ram8, ram12);
        }

        // ===== برچسب‌ها =====
        private async Task<(Tag New, Tag Popular)> SeedTagsAsync()
        {
            var newTag = new Tag(createBy: SystemUser, name: "جدید");
            newTag.Activate(SystemUser);

            var popular = new Tag(createBy: SystemUser, name: "پرفروش");
            popular.Activate(SystemUser);

            await _db.Tags.AddRangeAsync(newTag, popular);
            return (newTag, popular);
        }

        // ===== محصولات =====
        private async Task<(Product Samsung, Product Iphone)> SeedProductsAsync(
            (Category Mobile, Category Laptop) categories,
            (Brand Samsung, Brand Apple) brands,
            (ProductType SmartPhone, ProductType UltraBook) productTypes,
            (Tag New, Tag Popular) tags)
        {
            var samsungPhone = new Product(
                createBy: SystemUser,
                name: "سامسونگ گلکسی S24",
                shortDescription: "پرچمدار جدید سامسونگ با دوربین حرفه‌ای",
                description: "گوشی سامسونگ گلکسی S24 با طراحی مدرن و امکانات پیشرفته",
                categoryId: categories.Mobile.Id,
                productTypeId: productTypes.SmartPhone.Id,
                productCode: new ProductCode("PRD001"),
                slug: new Slug("samsung-galaxy-s24"),
                brandId: brands.Samsung.Id);
            samsungPhone.Activate(SystemUser);
            samsungPhone.AddTag(tags.New);
            samsungPhone.AddTag(tags.Popular);

            var iphone = new Product(
                createBy: SystemUser,
                name: "آیفون 15 پرو",
                shortDescription: "جدیدترین محصول اپل با تراشه A17",
                description: "آیفون 15 پرو با تراشه قدرتمند A17 Pro و دوربین پیشرفته",
                categoryId: categories.Mobile.Id,
                productTypeId: productTypes.SmartPhone.Id,
                productCode: new ProductCode("PRD002"),
                slug: new Slug("iphone-15-pro"),
                brandId: brands.Apple.Id);
            iphone.Activate(SystemUser);
            iphone.AddTag(tags.New);

            await _db.Products.AddRangeAsync(samsungPhone, iphone);
            return (samsungPhone, iphone);
        }

        // ===== واریانت‌ها =====
        private async Task SeedVariantsAsync()
        {
            var products = await _db.Products.ToListAsync();
            var samsungPhone = products[0];
            var iphone = products[1];

            var variant1 = new Variant(
                createBy: SystemUser,
                productId: samsungPhone.Id,
                price: 45000000m,
                stock: 10,
                sku: new SKU("S24-BLK-128"));
            variant1.Activate(SystemUser);
            samsungPhone.AddVariant(variant1);

            var variant2 = new Variant(
                createBy: SystemUser,
                productId: iphone.Id,
                price: 85000000m,
                stock: 5,
                sku: new SKU("IP15-PRO-256"),
                discountPercent: 10m);
            variant2.Activate(SystemUser);
            iphone.AddVariant(variant2);

            _db.Products.Update(samsungPhone);
            _db.Products.Update(iphone);
        }
    }
}