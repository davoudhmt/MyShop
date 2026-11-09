using API.Middlewares;
using Application.DTOs.Common;
using Application.DTOs.Identity.Person;
using Application.Interfaces.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers.Identity
{
    /// <summary>
    /// کنترلر مدیریت اشخاص (Persons) در پنل ادمین
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // دسترسی به این کنترلر نیازمند احراز هویت است
    public class PersonsController : ControllerBase
    {
        private readonly IPersonService _personService;

        /// <summary>
        /// سازنده کنترلر با تزریق سرویس مدیریت اشخاص
        /// </summary>
        public PersonsController(IPersonService personService)
        {
            _personService = personService;
        }

        /// <summary>
        /// دریافت لیست اشخاص با فیلتر و صفحه‌بندی
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<PersonDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPaged([FromQuery] PersonFilterDto filter)
        {
            var result = await _personService.GetPagedAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// دریافت شخص با شناسه
        /// </summary>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _personService.GetByIdAsync(id);
            if (result is null)
                return NotFound(new { message = $"شخص با شناسه {id} یافت نشد." });

            return Ok(result);
        }

        /// <summary>
        /// دریافت شخص بر اساس شماره موبایل
        /// </summary>
        [HttpGet("mobile/{mobile}")]
        [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByMobile(string mobile)
        {
            var result = await _personService.GetByMobileAsync(mobile);
            if (result is null)
                return NotFound(new { message = $"شخصی با شماره موبایل {mobile} یافت نشد." });

            return Ok(result);
        }

        /// <summary>
        /// ساخت شخص جدید
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(PersonDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreatePersonDto dto)
        {
            var result = await _personService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// به‌روزرسانی اطلاعات شخص
        /// </summary>
        [HttpPut("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(long id, [FromBody] UpdatePersonDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "شناسه در مسیر و بدنه درخواست یکسان نیست." });

            await _personService.UpdateAsync(dto);
            return Ok(new { message = "اطلاعات شخص با موفقیت به‌روزرسانی شد." });
        }

        /// <summary>
        /// حذف منطقی شخص
        /// </summary>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            await _personService.DeleteAsync(id);
            return Ok(new { message = "شخص با موفقیت حذف شد." });
        }

        /// <summary>
        /// فعال‌سازی شخص
        /// </summary>
        [HttpPut("{id:long}/activate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Activate(long id)
        {
            await _personService.ActivateAsync(id);
            return Ok(new { message = "شخص با موفقیت فعال شد." });
        }

        /// <summary>
        /// غیرفعال‌سازی شخص
        /// </summary>
        [HttpPut("{id:long}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(long id)
        {
            await _personService.DeactivateAsync(id);
            return Ok(new { message = "شخص با موفقیت غیرفعال شد." });
        }
    }
}