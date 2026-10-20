using Domain.Common;
using Domain.Constants;
using Domain.Entities.Location;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities.Identity
{
    /// <summary>
    /// موجودیت آدرس (یک آدرس در AddressBook کاربر)
    /// هر آدرس به یک Person متصل است و می‌تواند پیش‌فرض باشد
    /// </summary>
    public class Address : BaseEntity
    {
        /// <summary>
        /// عنوان آدرس (مثلاً "خانه"، "محل کار")
        /// </summary>
        public string Title { get; private set; } = string.Empty;

        /// <summary>
        /// آدرس دقیق پستی (خیابان، کوچه، پلاک، واحد)
        /// </summary>
        public string AddressDetail { get; private set; } = string.Empty;

        /// <summary>
        /// کد پستی ۱۰ رقمی
        /// </summary>
        public PostalCode PostalCode { get; private set; } = null!;

        /// <summary>
        /// شناسه شهر (Region با نوع City)
        /// از طریق این شناسه و ParentId، به استان نیز می‌رسیم
        /// </summary>
        public long RegionId { get; private set; }

        /// <summary>
        /// خاصیت ناوبری به Region (شهر)
        /// </summary>
        public Region Region { get; private set; } = null!;

        /// <summary>
        /// آیا این آدرس، آدرس پیش‌فرض شخص است؟
        /// </summary>
        public bool IsDefault { get; private set; } = false;

        /// <summary>
        /// کلید خارجی به Person
        /// </summary>
        public long PersonId { get; private set; }

        /// <summary>
        /// خاصیت ناوبری به Person
        /// </summary>
        public Person Person { get; private set; } = null!;

        /// <summary>
        /// سازنده پیش‌فرض برای EF Core
        /// </summary>
        protected Address() { }

        /// <summary>
        /// سازنده برای ایجاد آدرس جدید
        /// </summary>
        /// <param name="createBy">شناسه کاربر ایجادکننده</param>
        /// <param name="title">عنوان آدرس</param>
        /// <param name="addressDetail">آدرس دقیق پستی</param>
        /// <param name="postalCode">کد پستی ۱۰ رقمی</param>
        /// <param name="regionId">شناسه شهر</param>
        /// <param name="personId">شناسه Person</param>
        /// <param name="isDefault">آیا آدرس پیش‌فرض است؟ (پیش‌فرض: false)</param>
        public Address(
            long createBy,
            string title,
            string addressDetail,
            string postalCode,
            long regionId,
            long personId,
            bool isDefault = false) : base(createBy)
        {
            InitializeAddress(title, addressDetail, postalCode, regionId, personId, isDefault);
        }

        /// <summary>
        /// متد مقداردهی اولیه آدرس با استفاده از متدهای اعتبارسنجی
        /// </summary>
        /// <param name="title">عنوان آدرس</param>
        /// <param name="addressDetail">آدرس دقیق پستی</param>
        /// <param name="postalCode">کد پستی</param>
        /// <param name="regionId">شناسه شهر</param>
        /// <param name="personId">شناسه Person</param>
        /// <param name="isDefault">آیا آدرس پیش‌فرض است؟</param>
        private void InitializeAddress(string title, string addressDetail, string postalCode, long regionId, long personId, bool isDefault)
        {
            SetTitle(title);
            SetAddressDetail(addressDetail);
            SetPostalCode(postalCode);
            SetRegionId(regionId);
            SetPersonId(personId);
            SetIsDefault(isDefault);
        }

        // ===== متدهای اعتبارسنجی خصوصی =====

        /// <summary>
        /// تنظیم و اعتبارسنجی عنوان آدرس
        /// </summary>
        /// <param name="title">عنوان ورودی</param>
        /// <exception cref="DomainException">در صورت خالی بودن یا طول نامعتبر</exception>
        private void SetTitle(string title)
        {
            var normalized = title?.Trim();

            if (string.IsNullOrWhiteSpace(normalized))
                throw new DomainException("عنوان آدرس نمی‌تواند خالی باشد.", DomainConcept.Address, nameof(Title));

            if (Guard.IsInvalidLength(normalized, 2, 50))
                throw new DomainException("عنوان آدرس باید بین 2 تا 50 کاراکتر باشد.", DomainConcept.Address, nameof(Title));

            Title = normalized;
        }

        /// <summary>
        /// تنظیم و اعتبارسنجی آدرس دقیق پستی
        /// </summary>
        /// <param name="addressDetail">آدرس ورودی</param>
        /// <exception cref="DomainException">در صورت خالی بودن یا طول نامعتبر</exception>
        private void SetAddressDetail(string addressDetail)
        {
            var normalized = addressDetail?.Trim();

            if (string.IsNullOrWhiteSpace(normalized))
                throw new DomainException("آدرس پستی نمی‌تواند خالی باشد.", DomainConcept.Address, nameof(AddressDetail));

            if (Guard.IsInvalidLength(normalized, 10, 500))
                throw new DomainException("آدرس پستی باید بین 10 تا 500 کاراکتر باشد.", DomainConcept.Address, nameof(AddressDetail));

            AddressDetail = normalized;
        }

        /// <summary>
        /// تنظیم و اعتبارسنجی کد پستی
        /// </summary>
        /// <param name="postalCode">کد پستی ورودی</param>
        /// <exception cref="DomainException">در صورت نامعتبر بودن فرمت</exception>
        private void SetPostalCode(string postalCode)
        {
            PostalCode = PostalCode.Create(postalCode);
        }

        /// <summary>
        /// تنظیم و اعتبارسنجی شناسه شهر
        /// </summary>
        /// <param name="regionId">شناسه شهر</param>
        /// <exception cref="DomainException">در صورت نامعتبر بودن</exception>
        private void SetRegionId(long regionId)
        {
            if (Guard.IsInvalidId(regionId))
                throw new DomainException("شناسه شهر نامعتبر است.", DomainConcept.Address, nameof(RegionId));

            RegionId = regionId;
        }

        /// <summary>
        /// تنظیم و اعتبارسنجی شناسه Person
        /// </summary>
        /// <param name="personId">شناسه Person</param>
        /// <exception cref="DomainException">در صورت نامعتبر بودن</exception>
        private void SetPersonId(long personId)
        {
            if (Guard.IsInvalidId(personId))
                throw new DomainException("شناسه شخص نامعتبر است.", DomainConcept.Address, nameof(PersonId));

            PersonId = personId;
        }

        /// <summary>
        /// تنظیم وضعیت پیش‌فرض بودن آدرس
        /// </summary>
        /// <param name="isDefault">آیا آدرس پیش‌فرض است؟</param>
        private void SetIsDefault(bool isDefault)
        {
            IsDefault = isDefault;
        }

        // ===== متدهای به‌روزرسانی عمومی =====

        /// <summary>
        /// به‌روزرسانی عنوان آدرس
        /// </summary>
        /// <param name="title">عنوان جدید</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UpdateTitle(string title, long updateBy)
        {
            SetTitle(title);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// به‌روزرسانی آدرس دقیق پستی
        /// </summary>
        /// <param name="addressDetail">آدرس جدید</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UpdateAddressDetail(string addressDetail, long updateBy)
        {
            SetAddressDetail(addressDetail);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// به‌روزرسانی کد پستی
        /// </summary>
        /// <param name="postalCode">کد پستی جدید</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UpdatePostalCode(string postalCode, long updateBy)
        {
            SetPostalCode(postalCode);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// به‌روزرسانی شهر
        /// </summary>
        /// <param name="regionId">شناسه شهر جدید</param>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UpdateRegion(long regionId, long updateBy)
        {
            SetRegionId(regionId);
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// تنظیم این آدرس به عنوان آدرس پیش‌فرض
        /// توجه: منطق غیرفعال کردن سایر آدرس‌های پیش‌فرض در Service Layer انجام می‌شود
        /// </summary>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void MarkAsDefault(long updateBy)
        {
            IsDefault = true;
            UpdateModificationInfo(updateBy);
        }

        /// <summary>
        /// حذف این آدرس از وضعیت پیش‌فرض
        /// </summary>
        /// <param name="updateBy">شناسه کاربر به‌روزرسانی‌کننده</param>
        public void UnmarkAsDefault(long updateBy)
        {
            IsDefault = false;
            UpdateModificationInfo(updateBy);
        }
    }
}