using API.Middlewares;
using Application.DTOs.Catalog.Tag;
using Application.DTOs.Common;
using Application.Interfaces.Services.Catalog;
using Application.Services.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Catalog
{
    /// <summary>
    /// کنترلر مدیریت برچسب‌ها
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        /// <summary>
        /// سازنده کنترلر
        /// </summary>
        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        /// <summary>
        /// دریافت همه برچسب‌ها
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<TagDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _tagService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// دریافت برچسب‌ها با صفحه‌بندی و فیلتر
        /// </summary>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<TagDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPaged([FromQuery] TagFilterDto filter)
        {
            var result = await _tagService.GetPagedAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// دریافت برچسب با شناسه
        /// </summary>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(TagDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _tagService.GetByIdAsync(id);
            if (result is null)
                return NotFound(new { message = $"برچسب با شناسه {id} یافت نشد." });

            return Ok(result);
        }

        /// <summary>
        /// ساخت برچسب جدید
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(TagDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateTagDto dto)
        {
            var result = await _tagService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// به‌روزرسانی برچسب
        /// </summary>
        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(TagDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateTagDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "شناسه در مسیر و بدنه درخواست یکسان نیست." });

            var result = await _tagService.UpdateAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// حذف منطقی برچسب
        /// </summary>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            await _tagService.DeleteAsync(id);
            return Ok(new { message = "برچسب با موفقیت حذف شد." });
        }

        /// <summary>
        /// فعالسازی برچسب بر اساس شناسه
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id:long}/activate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Activate(long id)
        {
            await _tagService.ActivateAsync(id);
            return Ok(new { message = "برچسب با موفقیت فعال شد." });
        }

        /// <summary>
        /// غیرفعالسازی برچسب بر اساس شناسه
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id:long}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(long id)
        {
            await _tagService.DeactivateAsync(id);
            return Ok(new { message = "برچسب با موفقیت غیرفعال شد." });
        }
    }
}