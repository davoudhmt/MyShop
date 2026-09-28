using Application.DTOs.Catalog.Feature;
using Application.DTOs.Common;
using Application.Interfaces.Services.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Catalog  // ← یکدست با بقیه
{
    /// <summary>
    /// کنترلر مدیریت ویژگی‌ها
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FeaturesController : ControllerBase  // ← جمع
    {
        private readonly IFeatureService _featureService;

        public FeaturesController(IFeatureService featureService)
        {
            _featureService = featureService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<FeatureDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var features = await _featureService.GetAllAsync();
            return Ok(features);
        }

        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<FeatureDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged([FromQuery] FeatureFilterDto filter)
        {
            var result = await _featureService.GetPagedAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(FeatureDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(long id)
        {
            var feature = await _featureService.GetByIdAsync(id);
            if (feature is null)
                return NotFound(new { message = $"ویژگی با شناسه {id} یافت نشد." });

            return Ok(feature);
        }

        [HttpGet("by-product-type/{productTypeId:long}")]
        [ProducesResponseType(typeof(List<FeatureDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByProductTypeId(long productTypeId)
        {
            var features = await _featureService.GetByProductTypeIdAsync(productTypeId);
            return Ok(features);
        }

        [HttpPost]
        [ProducesResponseType(typeof(FeatureDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateFeatureDto dto)
        {
            try
            {
                var feature = await _featureService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = feature.Id }, feature);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateFeatureDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "شناسه در مسیر و بدنه درخواست یکسان نیست." });

            try
            {
                var result = await _featureService.UpdateAsync(dto);
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

        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                await _featureService.DeleteAsync(id);
                return Ok(new { message = "ویژگی با موفقیت حذف شد." });
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