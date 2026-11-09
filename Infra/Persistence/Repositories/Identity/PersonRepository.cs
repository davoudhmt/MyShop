using Application.DTOs.Common;
using Application.DTOs.Identity.Person;
using Application.Interfaces.Repositories.Identity;
using Domain.Entities.Identity;
using Domain.Enums;
using Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.Persistence.Repositories.Identity
{
    /// <summary>
    /// پیاده‌سازی ریپازیتوری شخص (Person) با استفاده از Entity Framework Core
    /// </summary>
    public class PersonRepository : IPersonRepository
    {
        private readonly MyShopDbContext _db;

        public PersonRepository(MyShopDbContext db)
        {
            _db = db;
        }

        public async Task<PagedResult<Person>> GetPagedAsync(PersonFilterDto filter)
        {
            var query = _db.Persons.AsNoTracking().AsQueryable();

            // ===== فیلتر جستجو در نام، نام خانوادگی یا موبایل =====
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.Trim();
                query = query.Where(p =>
                    (p.FirstName != null && p.FirstName.Contains(searchTerm)) ||
                    (p.LastName != null && p.LastName.Contains(searchTerm)) ||
                    p.Mobile.Value.Contains(searchTerm));
            }

            // ===== فیلتر وضعیت =====
            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                if (Enum.TryParse<EntityStatus>(filter.Status, true, out var status))
                {
                    query = query.Where(p => p.Status == status);
                }
            }

            var totalCount = await query.CountAsync();

            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var items = await query
                .OrderByDescending(p => p.CreateDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Person>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Person?> GetByMobileAsync(string mobile)
        {
            return await _db.Persons
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Mobile.Value == mobile);
        }

        public async Task<Person?> GetByIdAsync(long id)
        {
            return await _db.Persons
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Person person)
        {
            await _db.Persons.AddAsync(person);
        }

        public void Update(Person person)
        {
            _db.Persons.Update(person);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}