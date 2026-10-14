using Application.DTOs.Catalog.Feature;
using Application.DTOs.Common;
using Application.Interfaces.Services.Catalog;
using API.Middlewares;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Catalog
{
    /// <summary>
    /// کنترلر مدیریت ویژگی‌ها
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FeaturesController : ControllerBase
    {
        private readonly IFeatureService _featureService;

        /// <summary>
        /// سازنده کنترلر با تزریق سرویس ویژگی‌ها
        /// </summary>
        /// <param name="featureService"></param>
        public FeaturesController(IFeatureService featureService)
        {
            _featureService = featureService;
        }

        /// <summary>
        /// دریافت تمام ویژگی‌ها
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<FeatureDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var features = await _featureService.GetAllAsync();
            return Ok(features);
        }

        /// <summary>
        /// دریافت ویژگی‌ها به صورت صفحه‌بندی شده
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<FeatureDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPaged([FromQuery] FeatureFilterDto filter)
        {
            var result = await _featureService.GetPagedAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// دریافت ویژگی بر اساس شناسه
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(FeatureDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(long id)
        {
            var feature = await _featureService.GetByIdAsync(id);
            if (feature is null)
                return NotFound(new { message = $"ویژگی با شناسه {id} یافت نشد." });

            return Ok(feature);
        }

        /// <summary>
        /// دریافت ویژگی‌ها بر اساس شناسه نوع محصول
        /// </summary>
        /// <param name="productTypeId"></param>
        /// <returns></returns>
        [HttpGet("by-product-type/{productTypeId:long}")]
        [ProducesResponseType(typeof(List<FeatureDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByProductTypeId(long productTypeId)
        {
            var features = await _featureService.GetByProductTypeIdAsync(productTypeId);
            return Ok(features);
        }

        /// <summary>
        /// ایجاد ویژگی جدید
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(FeatureDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateFeatureDto dto)
        {
            var feature = await _featureService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = feature.Id }, feature);
        }

        /// <summary>
        /// به‌روزرسانی ویژگی موجود
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(FeatureDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateFeatureDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "شناسه در مسیر و بدنه درخواست یکسان نیست." });

            var result = await _featureService.UpdateAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// حذف ویژگی بر اساس شناسه
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            await _featureService.DeleteAsync(id);
            return Ok(new { message = "ویژگی با موفقیت حذف شد." });
        }
    }
}