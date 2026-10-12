using Application.DTOs.Catalog.Feature;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators.Catalog.Feature
{
    /// <summary>
    /// اعتبارسنجی DTO به‌روزرسانی ویژگی
    /// </summary>
    public class UpdateFeatureValidator : AbstractValidator<UpdateFeatureDto>
    {
        public UpdateFeatureValidator()
        {
            // شناسه: باید معتبر باشد
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("شناسه ویژگی نامعتبر است.");

            // نام ویژگی: همان قوانین Create
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("نام ویژگی نمی‌تواند خالی باشد.")
                .Length(2, 100).WithMessage("نام ویژگی باید بین 2 تا 100 کاراکتر باشد.");

            // چندگانگی: باید یک مقدار معتبر از Enum باشد
            RuleFor(x => x.Multiplicity)
                .IsInEnum().WithMessage("مقدار چندگانگی ویژگی نامعتبر است.");

            // وضعیت: باید یکی از مقادیر معتبر EntityStatus باشد
            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("وضعیت ویژگی نمی‌تواند خالی باشد.")
                .Must(BeValidStatus).WithMessage("وضعیت ویژگی باید 'Active' یا 'Inactive' باشد.");
        }

        /// <summary>
        /// بررسی معتبر بودن مقدار وضعیت
        /// </summary>
        private bool BeValidStatus(string status)
        {
            return Enum.TryParse<EntityStatus>(status, true, out _);
        }
    }
}