using Application.DTOs.Catalog.Brand;
using Application.DTOs.Common;
using Application.Interfaces.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Catalog
{
    /// <summary>
    /// کنترلر مدیریت برندها
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        /// <summary>
        /// سازنده کلاس
        /// </summary>
        /// <param name="brandService">سرویس برند</param>
        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        /// <summary>
        /// دریافت همه برندها
        /// </summary>
        /// <returns>لیست برندها</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<BrandDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _brandService.GetAllAsync();
            return Ok(brands);
        }

        /// <summary>
        /// دریافت برندها با صفحه‌بندی، جستجو و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر</param>
        /// <returns>نتیجه صفحه‌بندی شده</returns>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<BrandDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged([FromQuery] BrandFilterDto filter)
        {
            var result = await _brandService.GetPagedAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// دریافت برند با شناسه
        /// </summary>
        /// <param name="id">شناسه برند</param>
        /// <returns>برند یا 404</returns>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(BrandDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(long id)
        {
            var brand = await _brandService.GetByIdAsync(id);
            if (brand is null)
                return NotFound(new { message = $"برند با شناسه {id} یافت نشد." });

            return Ok(brand);
        }

        /// <summary>
        /// ساخت برند جدید
        /// </summary>
        /// <param name="dto">اطلاعات برند جدید</param>
        /// <returns>شناسه برند ساخته‌شده</returns>
        [HttpPost]
        [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateBrandDto dto)
        {
            try
            {
                var brandId = await _brandService.CreateAsync(dto);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = brandId },
                    new { id = brandId });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// به‌روزرسانی برند
        /// </summary>
        /// <param name="id">شناسه برند</param>
        /// <param name="dto">اطلاعات برند برای به‌روزرسانی</param>
        [HttpPut("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateBrandDto dto)
        {
            // بررسی تطابق شناسه در مسیر و بدنه درخواست
            if (id != dto.Id)
                return BadRequest(new { message = "شناسه در مسیر و بدنه درخواست یکسان نیست." });

            try
            {
                await _brandService.UpdateAsync(dto);
                return NoContent();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("یافت نشد"))
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// حذف برند
        /// </summary>
        /// <param name="id">شناسه برند</param>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                await _brandService.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}