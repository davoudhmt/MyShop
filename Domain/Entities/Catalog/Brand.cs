using Domain.Common;
using Domain.Constants;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities.Catalog
{
    /// <summary>
    /// کلاس Brand
    /// </summary>
    public class Brand : BaseEntity
    {
        /// <summary>
        /// فیلد نام برند
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// فیلد توضیحات برند
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// فیلد تصویر برند
        /// </summary>
        public ImageUrl? ImageUrl { get; private set; }

        /// <summary>
        /// سازنده پیشفرض Brand برای EF Core
        /// </summary>
        protected Brand() { }

        /// <summary>
        /// سازنده Brand
        /// </summary>
        /// <param name="createBy"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="brandImageUrl"></param>
        public Brand(long createBy, string name, string? description = null, ImageUrl? imageUrl = null) : base(createBy)
        {
            InitializeBrand(name, description, imageUrl);
        }

        /// <summary>
        /// مقداردهی اولیه فیلدهای برند
        /// این متد فقط توسط سازنده فراخوانی می شود
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="brandImageUrl"></param>
        private void InitializeBrand(string name, string? description, ImageUrl? imageUrl)
        {
            SetName(name);
            SetDescription(description);
            ImageUrl = imageUrl;
        }

        /// <summary>
        /// متد اعتبارسنجی نام Brand
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="DomainException"></exception>
        private void SetName(string name)
        {
            var normalizedName = name?.Trim();

            if (string.IsNullOrWhiteSpace(normalizedName))
                throw new DomainException("نام برند نمی تواند خالی باشد", DomainConcept.Brand, nameof(Name));

            if (Guard.IsInvalidLength(normalizedName, 2, 100))
                throw new DomainException("نام برند باید بین 2 تا 100 کاراکتر باشد.", DomainConcept.Brand, nameof(Name));

            Name = normalizedName;
        }

        /// <summary>
        /// متد اعتبارسنجی توضیحات Brand
        /// </summary>
        /// <param name="description"></param>
        /// <exception cref="DomainException"></exception>
        private void SetDescription(string? description)
        {
            var normalizedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

            if (Guard.IsOverMaxLength(normalizedDescription, 500))
                throw new DomainException("توضیحات برند نمی تواند بیش از 500 کاراکتر باشد.", DomainConcept.Brand, nameof(Description));

            Description = normalizedDescription;
        }

        /// <summary>
        /// متد به‌روزرسانی نام برند
        /// </summary>
        /// <param name="name"></param>
        /// <param name="updateBy"></param>
        public void UpdateBrandName(string name, long updateBy)
        {
            SetName(name);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// متد به‌روزرسانی توضیحات برند
        /// </summary>
        /// <param name="description"></param>
        /// <param name="updateBy"></param>
        public void UpdateBrandDescription(string? description, long updateBy)
        {
            SetDescription(description);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// متد به‌روزرسانی تصویر برند
        /// </summary>
        /// <param name="brandImageUrl"></param>
        /// <param name="updateBy"></param>
        public void UpdateBrandImageUrl(ImageUrl? brandImageUrl, long updateBy)
        {
            ImageUrl = brandImageUrl;
            UpdateModificationInfo(updateBy);
        }
    }
}
