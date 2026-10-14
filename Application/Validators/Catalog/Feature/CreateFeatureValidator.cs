using Application.DTOs.Catalog.Feature;
using FluentValidation;

namespace Application.Validators.Catalog.Feature
{
    /// <summary>
    /// اعتبارسنجی DTO ساخت ویژگی جدید
    /// </summary>
    public class CreateFeatureValidator : AbstractValidator<CreateFeatureDto>
    {
        /// <summary>
        /// سازنده اعتبارسنجی
        /// </summary>
        public CreateFeatureValidator()
        {
            // نام ویژگی: اجباری، بین 2 تا 100 کاراکتر (مطابق دامنه)
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("نام ویژگی نمی‌تواند خالی باشد.")
                .Length(2, 100).WithMessage("نام ویژگی باید بین 2 تا 100 کاراکتر باشد.");

            // چندگانگی: باید یک مقدار معتبر از Enum باشد
            // نکته: مدل بایندر ASP.NET Core هم این را چک می‌کند، اما این خط برای پیام خطای فارسی و دفاع در عمق اضافه شده است.
            RuleFor(x => x.Multiplicity)
                .IsInEnum().WithMessage("مقدار چندگانگی ویژگی نامعتبر است.");
        }
    }
}