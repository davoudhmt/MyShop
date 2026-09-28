using Application.DTOs.Catalog.Tag;
using Application.DTOs.Common;
using Application.Interfaces.Catalog;
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateTagDto dto)
        {
            try
            {
                var result = await _tagService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// به‌روزرسانی برچسب
        /// </summary>
        [HttpPut("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateTagDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "شناسه در مسیر و بدنه درخواست یکسان نیست." });

            try
            {
                var result = await _tagService.UpdateAsync(dto);
                return Ok(result);
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
        /// حذف منطقی برچسب
        /// </summary>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                await _tagService.DeleteAsync(id);
                return Ok(new { message = "برچسب با موفقیت حذف شد." });
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
    }
}