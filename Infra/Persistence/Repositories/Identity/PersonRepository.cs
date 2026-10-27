using Application.Interfaces.Repositories.Identity;
using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistence.Repositories.Identity
{
    /// <summary>
    /// پیاده‌سازی ریپازیتوری شخص (Person) با استفاده از Entity Framework Core
    /// </summary>
    public class PersonRepository : IPersonRepository
    {
        private readonly MyShopDbContext _context;

        /// <summary>
        /// سازنده ریپازیتوری شخص
        /// </summary>
        /// <param name="context">کانتکست دیتابیس</param>
        public PersonRepository(MyShopDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// دریافت شخص بر اساس شماره موبایل
        /// </summary>
        public async Task<Person?> GetByMobileAsync(string mobile)
        {
            return await _context.Persons
                .FirstOrDefaultAsync(p => p.Mobile.Value == mobile);
        }

        /// <summary>
        /// دریافت شخص بر اساس شناسه
        /// </summary>
        public async Task<Person?> GetByIdAsync(long id)
        {
            return await _context.Persons.FindAsync(id);
        }

        /// <summary>
        /// افزودن شخص جدید
        /// </summary>
        public async Task AddAsync(Person person)
        {
            await _context.Persons.AddAsync(person);
        }

        /// <summary>
        /// به‌روزرسانی شخص
        /// </summary>
        public void Update(Person person)
        {
            _context.Persons.Update(person);
        }

        /// <summary>
        /// ذخیره تغییرات در دیتابیس
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}