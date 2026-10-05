using Application.DTOs.Catalog.FeatureValue;
using Application.DTOs.Common;
using Application.Interfaces.Services.Catalog;
using API.Middlewares;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Catalog
{
    /// <summary>
    /// کنترلر مدیریت مقادیر ویژگی
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FeatureValuesController : ControllerBase
    {
        private readonly IFeatureValueService _featureValueService;

        /// <summary>
        /// سازنده کنترلر
        /// </summary>
        public FeatureValuesController(IFeatureValueService featureValueService)
        {
            _featureValueService = featureValueService;
        }

        /// <summary>
        /// دریافت همه مقادیر ویژگی
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<FeatureValueDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _featureValueService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// دریافت مقادیر ویژگی با صفحه‌بندی و فیلتر
        /// </summary>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<FeatureValueDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPaged([FromQuery] FeatureValueFilterDto filter)
        {
            var result = await _featureValueService.GetPagedAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// دریافت مقادیر یک ویژگی خاص
        /// </summary>
        [HttpGet("by-feature/{featureId:long}")]
        [ProducesResponseType(typeof(List<FeatureValueDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByFeatureId(long featureId)
        {
            var result = await _featureValueService.GetByFeatureIdAsync(featureId);
            return Ok(result);
        }

        /// <summary>
        /// دریافت مقدار ویژگی با شناسه
        /// </summary>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(FeatureValueDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _featureValueService.GetByIdAsync(id);
            if (result is null)
                return NotFound(new { message = $"مقدار ویژگی با شناسه {id} یافت نشد." });

            return Ok(result);
        }

        /// <summary>
        /// ساخت مقدار ویژگی جدید
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(FeatureValueDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateFeatureValueDto dto)
        {
            var result = await _featureValueService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// به‌روزرسانی مقدار ویژگی
        /// </summary>
        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(FeatureValueDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateFeatureValueDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "شناسه در مسیر و بدنه درخواست یکسان نیست." });

            var result = await _featureValueService.UpdateAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// حذف منطقی مقدار ویژگی
        /// </summary>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            await _featureValueService.DeleteAsync(id);
            return Ok(new { message = "مقدار ویژگی با موفقیت حذف شد." });
        }
    }
}