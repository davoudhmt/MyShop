using Application.DTOs.Common;
using Application.DTOs.Identity.Person;
using Application.Interfaces.Repositories.Identity;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Identity;
using Application.Mappings.Identity;
using Domain.Constants;
using Domain.Entities.Identity;
using Domain.Exceptions;
using System.Threading.Tasks;

namespace Application.Services.Identity
{
    /// <summary>
    /// پیاده‌سازی سرویس مدیریت اشخاص (Person)
    /// این سرویس مسئولیت مدیریت چرخه حیات اطلاعات هویتی و تماس اشخاص را بر عهده دارد.
    /// </summary>
    public class PersonService : IPersonService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IPersonRepository _personRepository;

        /// <summary>
        /// سازنده سرویس شخص و مقداردهی وابستگی‌ها
        /// </summary>
        public PersonService(
            ICurrentUserService currentUserService,
            IPersonRepository personRepository)
        {
            _currentUserService = currentUserService;
            _personRepository = personRepository;
        }

        /// <summary>
        /// دریافت لیست صفحه‌بندی شده اشخاص بر اساس فیلتر
        /// </summary>
        public async Task<PagedResult<PersonDto>> GetPagedAsync(PersonFilterDto filter)
        {
            var pagedPersons = await _personRepository.GetPagedAsync(filter);

            return new PagedResult<PersonDto>
            {
                // استفاده از ToAdminDtoList برای نگاشت مخصوص پنل ادمین
                Items = pagedPersons.Items.ToAdminDtoList(),
                PageNumber = pagedPersons.PageNumber,
                PageSize = pagedPersons.PageSize,
                TotalCount = pagedPersons.TotalCount
            };
        }

        /// <summary>
        /// دریافت شخص با شناسه
        /// </summary>
        public async Task<PersonDto?> GetByIdAsync(long id)
        {
            var person = await _personRepository.GetByIdAsync(id);
            return person?.ToAdminDto();
        }

        /// <summary>
        /// دریافت شخص بر اساس شماره موبایل
        /// </summary>
        public async Task<PersonDto?> GetByMobileAsync(string mobile)
        {
            var person = await _personRepository.GetByMobileAsync(mobile);
            return person?.ToAdminDto();
        }

        /// <summary>
        /// ساخت شخص جدید
        /// </summary>
        public async Task<PersonDto> CreateAsync(CreatePersonDto dto)
        {
            // بررسی تکراری نبودن شماره موبایل
            var existingPerson = await _personRepository.GetByMobileAsync(dto.Mobile);
            if (existingPerson is not null)
            {
                throw new ConflictException(
                    $"شخصی با شماره موبایل '{dto.Mobile}' قبلاً ثبت شده است.",
                    DomainConcept.Person,
                    nameof(Person.Mobile));
            }

            var person = new Person(
                createBy: _currentUserService.UserId,
                mobile: dto.Mobile,
                firstName: dto.FirstName,
                lastName: dto.LastName,
                landline: dto.Landline,
                email: dto.Email);

            await _personRepository.AddAsync(person);
            await _personRepository.SaveChangesAsync();

            return person.ToAdminDto();
        }

        /// <summary>
        /// به‌روزرسانی اطلاعات شخص
        /// </summary>
        public async Task UpdateAsync(UpdatePersonDto dto)
        {
            var person = await _personRepository.GetByIdAsync(dto.Id);
            if (person is null)
            {
                throw new NotFoundException(
                    $"شخص با شناسه {dto.Id} یافت نشد.",
                    DomainConcept.Person,
                    nameof(Person.Id));
            }

            // اگر شماره موبایل تغییر کرده است، بررسی کنیم تکراری نباشد
            if (person.Mobile.Value != dto.Mobile)
            {
                var existingByMobile = await _personRepository.GetByMobileAsync(dto.Mobile);
                if (existingByMobile is not null && existingByMobile.Id != dto.Id)
                {
                    throw new ConflictException(
                        $"شماره موبایل '{dto.Mobile}' متعلق به شخص دیگری است.",
                        DomainConcept.Person,
                        nameof(Person.Mobile));
                }
            }

            // فراخوانی متدهای به‌روزرسانی در Entity
            person.UpdateFirstName(dto.FirstName, _currentUserService.UserId);
            person.UpdateLastName(dto.LastName, _currentUserService.UserId);
            person.UpdateMobile(dto.Mobile, _currentUserService.UserId);
            person.UpdateLandline(dto.Landline, _currentUserService.UserId);
            person.UpdateEmail(dto.Email, _currentUserService.UserId);

            _personRepository.Update(person);
            await _personRepository.SaveChangesAsync();
        }

        /// <summary>
        /// حذف منطقی شخص
        /// </summary>
        public async Task DeleteAsync(long id)
        {
            var person = await _personRepository.GetByIdAsync(id);
            if (person is null)
            {
                throw new NotFoundException(
                    $"شخص با شناسه {id} یافت نشد.",
                    DomainConcept.Person,
                    nameof(Person.Id));
            }

            person.Delete(_currentUserService.UserId);
            _personRepository.Update(person);
            await _personRepository.SaveChangesAsync();
        }

        /// <summary>
        /// فعال‌سازی شخص
        /// </summary>
        public async Task ActivateAsync(long id)
        {
            var person = await _personRepository.GetByIdAsync(id);
            if (person is null)
            {
                throw new NotFoundException(
                    $"شخص با شناسه {id} یافت نشد.",
                    DomainConcept.Person,
                    nameof(Person.Id));
            }

            person.Activate(_currentUserService.UserId);
            _personRepository.Update(person);
            await _personRepository.SaveChangesAsync();
        }

        /// <summary>
        /// غیرفعال‌سازی شخص
        /// </summary>
        public async Task DeactivateAsync(long id)
        {
            var person = await _personRepository.GetByIdAsync(id);
            if (person is null)
            {
                throw new NotFoundException(
                    $"شخص با شناسه {id} یافت نشد.",
                    DomainConcept.Person,
                    nameof(Person.Id));
            }

            person.Deactivate(_currentUserService.UserId);
            _personRepository.Update(person);
            await _personRepository.SaveChangesAsync();
        }
    }
}