using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities.Catalog
{
    /// <summary>
    /// کلاس Category (دسته‌بندی محصولات)
    /// </summary>
    public class Category : BaseEntity
    {
        /// <summary>
        /// حداکثر عمق مجاز برای ساختار درختی (شامل ریشه)
        /// مثال: با MaxDepth = 3، حداکثر ۳ سطح زیرمجموعه مجاز است
        /// </summary>
        public const int MaxDepth = 3;

        /// <summary>
        /// فیلد نام دسته‌بندی
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// فیلد توضیحات دسته‌بندی
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// فیلد شناسه دسته‌بندی والد (برای سلسله‌مراتب)
        /// </summary>
        public long? ParentCategoryId { get; private set; }

        /// <summary>
        /// فیلد تصویر دسته‌بندی
        /// </summary>
        public ImageUrl? ImageUrl { get; private set; }

        /// <summary>
        /// فیلد کد دسته‌بندی
        /// </summary>
        public CategoryCode CategoryCode { get; private set; } = null!;

        /// <summary>
        /// لیست داخلی انواع محصولات این دسته‌بندی
        /// </summary>
        private readonly List<ProductType> _productTypes = new();

        /// <summary>
        /// انواع محصولات این دسته‌بندی (فقط خواندنی)
        /// </summary>
        public IReadOnlyCollection<ProductType> ProductTypes => _productTypes.AsReadOnly();

        /// <summary>
        /// لیست داخلی فرزندان (زیردسته‌ها)
        /// </summary>
        private readonly List<Category> _children = new();

        /// <summary>
        /// لیست فرزندان (فقط خواندنی)
        /// </summary>
        public IReadOnlyCollection<Category> Children => _children.AsReadOnly();

        /// <summary>
        /// Navigation Property برای دسته پدر
        /// </summary>
        public Category? Parent { get; private set; }

        /// <summary>
        /// آیا این دسته ریشه است؟ (پدر ندارد)
        /// </summary>
        public bool IsRoot => !ParentCategoryId.HasValue;

        /// <summary>
        /// آیا این دسته فرزند است؟
        /// </summary>
        public bool IsChild => ParentCategoryId.HasValue;

        /// <summary>
        /// سازنده پیش‌فرض EF Core
        /// </summary>
        protected Category() { }

        /// <summary>
        /// سازنده کلاس دسته‌بندی
        /// </summary>
        public Category(
            long createBy,
            string name,
            CategoryCode categoryCode,
            string? description = null,
            ImageUrl? imageUrl = null,
            long? parentCategoryId = null) : base(createBy)
        {
            InitializeCategory(name, categoryCode, description, imageUrl, parentCategoryId);
        }

        /// <summary>
        /// متد مقداردهی اولیه دسته‌بندی
        /// </summary>
        private void InitializeCategory(
            string name,
            CategoryCode categoryCode,
            string? description,
            ImageUrl? imageUrl,
            long? parentCategoryId)
        {
            SetName(name);
            SetCategoryCode(categoryCode);
            SetDescription(description);
            SetImageUrl(imageUrl);
            SetParentCategoryId(parentCategoryId);
        }

        // ===== متدهای Set (اعتبارسنجی) =====

        private void SetName(string name)
        {
            var normalizedName = name?.Trim();

            if (string.IsNullOrWhiteSpace(normalizedName))
                throw new DomainException("نام دسته‌بندی نمی‌تواند خالی باشد.", DomainConcept.Category, nameof(Name));

            if (Guard.IsInvalidLength(normalizedName, 2, 100))
                throw new DomainException("نام دسته‌بندی باید بین 2 تا 100 کاراکتر باشد.", DomainConcept.Category, nameof(Name));

            Name = normalizedName!;
        }

        private void SetCategoryCode(CategoryCode categoryCode)
        {
            if (categoryCode is null)
                throw new DomainException("کد دسته‌بندی نمی‌تواند خالی باشد.", DomainConcept.Category, nameof(CategoryCode));

            CategoryCode = categoryCode;
        }

        private void SetDescription(string? description)
        {
            var normalizedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

            if (Guard.IsOverMaxLength(normalizedDescription, 500))
                throw new DomainException("توضیحات دسته‌بندی نمی‌تواند بیش از 500 کاراکتر باشد.", DomainConcept.Category, nameof(Description));

            Description = normalizedDescription;
        }

        private void SetImageUrl(ImageUrl? imageUrl)
        {
            ImageUrl = imageUrl;
        }

        private void SetParentCategoryId(long? parentCategoryId)
        {
            if (parentCategoryId.HasValue && parentCategoryId.Value <= 0)
                throw new DomainException("شناسه دسته‌بندی والد نامعتبر است.", DomainConcept.Category, nameof(ParentCategoryId));

            if (parentCategoryId.HasValue && parentCategoryId.Value == Id)
                throw new DomainException("دسته‌بندی نمی‌تواند والد خودش باشد.", DomainConcept.Category, nameof(ParentCategoryId));

            ParentCategoryId = parentCategoryId;
        }

        // ===== متدهای Update =====

        public void UpdateCategoryName(string name, long updateBy)
        {
            SetName(name);
            UpdateModificationInfo(updateBy);
        }

        public void UpdateCategoryDescription(string? description, long updateBy)
        {
            SetDescription(description);
            UpdateModificationInfo(updateBy);
        }

        public void UpdateCategoryImageUrl(ImageUrl? imageUrl, long updateBy)
        {
            SetImageUrl(imageUrl);
            UpdateModificationInfo(updateBy);
        }

        public void UpdateCategoryCode(CategoryCode categoryCode, long updateBy)
        {
            SetCategoryCode(categoryCode);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// تبدیل به ریشه (حذف پدر)
        /// </summary>
        public void MakeRootCategory(long updateBy)
        {
            SetParentCategoryId(null);
            UpdateModificationInfo(updateBy);
        }

        // ===== مدیریت ساختار درختی =====

        /// <summary>
        /// افزودن فرزند (زیردسته) به این دسته
        /// شامل بررسی چرخه و محدودیت عمق
        /// </summary>
        public void AddChild(Category child)
        {
            if (child is null)
                throw new DomainException("فرزند نمی‌تواند خالی باشد.", DomainConcept.Category, nameof(Children));

            // چک ۱: چرخه - این دسته نباید فرزند یا نواده‌ی child باشد
            if (child == this || IsDescendantOf(child))
                throw new DomainException(
                    "این کار باعث ایجاد چرخه در ساختار درختی می‌شود.",
                    DomainConcept.Category,
                    nameof(Children));

            // چک ۲: عمق - آیا عمق جدید از حد مجاز بیشتر می‌شود؟
            var newChildDepth = GetDepth() + 1 + child.GetMaxSubtreeDepth();
            if (newChildDepth > MaxDepth)
                throw new DomainException(
                    $"حداکثر عمق مجاز برای ساختار درختی {MaxDepth} سطح است.",
                    DomainConcept.Category,
                    nameof(Children));

            // چک ۳: فرزند نباید پدر دیگری داشته باشد
            if (child.ParentCategoryId.HasValue && child.ParentCategoryId != Id)
                throw new DomainException(
                    "این دسته‌بندی قبلاً فرزند دسته دیگری است.",
                    DomainConcept.Category,
                    nameof(Children));

            if (!_children.Contains(child))
            {
                _children.Add(child);
                child.SetAsChildOf(Id);
            }
        }

        /// <summary>
        /// انتقال این دسته به زیر یک پدر جدید
        /// شامل بررسی چرخه و محدودیت عمق
        /// </summary>
        /// <param name="newParent">پدر جدید (null برای تبدیل به ریشه)</param>
        /// <param name="updateBy">شناسه کاربر</param>
        public void MoveTo(Category? newParent, long updateBy)
        {
            // اگه null باشه، تبدیل به ریشه می‌شه
            if (newParent is null)
            {
                MakeRootCategory(updateBy);
                return;
            }

            // چک ۱: چرخه - پدر جدید نباید فرزند یا نواده‌ی این دسته باشد
            if (newParent == this || newParent.IsDescendantOf(this))
                throw new DomainException(
                    "این کار باعث ایجاد چرخه در ساختار درختی می‌شود.",
                    DomainConcept.Category,
                    nameof(ParentCategoryId));

            // چک ۲: عمق - محاسبه عمق جدید این دسته و زیردرختش
            var newMyDepth = newParent.GetDepth() + 1;
            var newTotalDepth = newMyDepth + GetMaxSubtreeDepth();
            if (newTotalDepth > MaxDepth)
                throw new DomainException(
                    $"با این انتقال، عمق از حد مجاز ({MaxDepth}) بیشتر می‌شود.",
                    DomainConcept.Category,
                    nameof(ParentCategoryId));

            SetParentCategoryId(newParent.Id);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// حذف فرزند از این دسته (تبدیل به ریشه)
        /// </summary>
        public void RemoveChild(Category child)
        {
            if (_children.Remove(child))
            {
                child.SetAsRoot();
            }
        }

        // ===== متدهای internal =====

        internal void SetAsChildOf(long parentId)
        {
            if (parentId <= 0)
                throw new DomainException("شناسه پدر نامعتبر است.", DomainConcept.Category, nameof(ParentCategoryId));

            ParentCategoryId = parentId;
        }

        internal void SetAsRoot()
        {
            ParentCategoryId = null;
        }

        // ===== متدهای محاسباتی برای ساختار درختی =====

        /// <summary>
        /// محاسبه عمق این دسته در درخت (ریشه = 0)
        /// </summary>
        public int GetDepth()
        {
            int depth = 0;
            var current = Parent;
            while (current is not null)
            {
                depth++;
                current = current.Parent;
            }
            return depth;
        }

        /// <summary>
        /// محاسبه عمق درخت زیر این دسته (شامل خودش)
        /// برای بررسی محدودیت عمق هنگام جابجایی
        /// </summary>
        public int GetMaxSubtreeDepth()
        {
            if (!_children.Any())
                return 1;

            return 1 + _children.Max(c => c.GetMaxSubtreeDepth());
        }

        /// <summary>
        /// بررسی می‌کند آیا این دسته، فرزند (یا نواده) دسته مشخص شده است؟
        /// برای جلوگیری از ایجاد چرخه در ساختار درختی
        /// </summary>
        public bool IsDescendantOf(Category ancestor)
        {
            if (ancestor is null)
                return false;

            if (ParentCategoryId == ancestor.Id)
                return true;

            return Parent is not null && Parent.IsDescendantOf(ancestor);
        }

        // ===== حذف منطقی =====

        /// <summary>
        /// حذف منطقی دسته‌بندی
        /// قانون: دسته‌بندی که زیردسته فعال دارد، قابل حذف نیست
        /// </summary>
        public new void Delete(long updateBy)
        {
            var hasActiveChildren = _children.Any(c => !c.IsDeleted);

            if (hasActiveChildren)
                throw new DomainException(
                    "دسته‌بندی دارای زیردسته فعال است. ابتدا زیردسته‌ها را حذف کنید.",
                    DomainConcept.Category,
                    nameof(Children));

            base.Delete(updateBy);
        }

        // ===== مدیریت ProductType (رابطه چند به چند) =====

        public void AddProductType(ProductType productType)
        {
            if (productType is null)
                throw new DomainException("نوع محصول نمی‌تواند خالی باشد.", DomainConcept.Category, nameof(ProductTypes));

            if (!_productTypes.Contains(productType))
            {
                _productTypes.Add(productType);
                productType.AddCategory(this);
            }
        }

        public void RemoveProductType(ProductType productType)
        {
            if (_productTypes.Remove(productType))
                productType.RemoveCategory(this);
        }
    }
}