using Domain.Enums;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common
{
    /// <summary>
    /// کلاس پایه برای موجودیت‌ها
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// فیلد شناسه یکتا
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// فیلد تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// فیلد تاریخ آخرین به‌روزرسانی
        /// </summary>
        public DateTime? UpdateDate { get; private set; }

        /// <summary>
        /// فیلد وضعیت
        /// </summary>
        public EntityStatus Status { get; private set; } = EntityStatus.Active;

        /// <summary>
        /// فیلد شناسه کاربر ایجادکننده
        /// </summary>
        public long CreateBy { get; private set; }

        /// <summary>
        /// فیلد شناسه کاربر آخرین به‌روزرسانی
        /// </summary>
        public long? UpdateBy { get; private set; }

        /// <summary>
        /// فیلد ردیابی نسخه برای کنترل همزمانی
        /// </summary>
        //[Timestamp]
        public byte[]? RowVersion { get; private set; }

        /// <summary>
        /// مقدار نشان‌دهنده حذف منطقی موجودیت
        /// </summary>
        public bool IsDeleted => Status == EntityStatus.Deleted;

        /// <summary>
        /// مقدار نشان‌دهنده فعال بودن موجودیت
        /// </summary>
        public bool IsActive => Status == EntityStatus.Active;

        /// <summary>
        /// سازنده پیشفرض برای EF Core
        /// </summary>
        protected BaseEntity() { }

        /// <summary>
        /// سازنده با پارامتر شناسه کاربر ایجادکننده
        /// </summary>
        /// <param name="createBy"></param>
        protected BaseEntity(long createBy)
        {
            Initialize(createBy);
        }

        /// <summary>
        /// متد مقداردهی اولیه موجودیت با شناسه کاربر ایجادکننده
        /// </summary>
        /// <param name="createBy"></param>
        /// <exception cref="DomainException"></exception>
        private void Initialize(long createBy)
        {
            if (Guard.IsInvalidId(createBy))
                throw new DomainException("شناسه کاربری نامعتبر است.");
            CreateBy = createBy;
        }

        /// <summary>
        /// به‌روزرسانی اطلاعات موجودیت
        /// </summary>
        /// <param name="updateBy"></param>
        /// <exception cref="DomainException"></exception>
        protected void UpdateModificationInfo(long updateBy)
        {
            if (Guard.IsInvalidId(updateBy))
                throw new DomainException("شناسه کاربری نامعتبر است.");
            UpdateBy = updateBy;
            UpdateDate = DateTime.UtcNow;
        }

        /// <summary>
        /// به‌روزرسانی وضعیت موجودیت
        /// </summary>
        /// <param name="status"></param>
        /// <param name="updateBy"></param>
        private void ChangeStatus(EntityStatus status, long updateBy)
        {
            if (Guard.IsInvalidEnum(status))
                throw new DomainException("وضعیت نامعتبر است.");

            UpdateModificationInfo(updateBy);
            Status = status;
        }

        /// <summary>
        /// حذف موجودیت
        /// </summary>
        /// <param name="updateBy"></param>
        public virtual void Delete(long updateBy)
        {
            ChangeStatus(EntityStatus.Deleted, updateBy);
        }

        /// <summary>
        /// فعال‌سازی موجودیت
        /// </summary>
        /// <param name="updateBy"></param>
        public virtual void Activate(long updateBy)
        {
            ChangeStatus(EntityStatus.Active, updateBy);
        }

        /// <summary>
        /// غیرفعال‌سازی موجودیت
        /// </summary>
        /// <param name="updateBy"></param>
        public virtual void Deactivate(long updateBy)
        {
            ChangeStatus(EntityStatus.Inactive, updateBy);
        }
    }
}
