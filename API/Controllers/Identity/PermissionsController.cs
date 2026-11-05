using API.Middlewares;
using Application.DTOs.Common;
using Application.DTOs.Identity.Permission;
using Application.Interfaces.Services.Identity;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Identity
{
    /// <summary>
    /// کنترلر مدیریت مجوزها (Permissions)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // دسترسی به این کنترلر نیازمند احراز هویت است
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        /// <summary>
        /// سازنده کنترلر با تزریق سرویس مدیریت مجوزها
        /// </summary>
        /// <param name="permissionService">سرویس مدیریت مجوزها</param>
        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        /// <summary>
        /// دریافت لیست مجوزها با فیلتر و صفحه‌بندی
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<PermissionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPaged([FromQuery] PermissionFilterDto filter)
        {
            var result = await _permissionService.GetPagedAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// دریافت مجوز با شناسه
        /// </summary>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(PermissionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _permissionService.GetByIdAsync(id);
            if (result is null)
                return NotFound(new { message = $"مجوز با شناسه {id} یافت نشد." });

            return Ok(result);
        }

        /// <summary>
        /// دریافت مجوزها به صورت گروه‌بندی شده بر اساس ماژول (مناسب برای پنل ادمین)
        /// </summary>
        [HttpGet("grouped-by-module")]
        [ProducesResponseType(typeof(Dictionary<Module, IReadOnlyList<PermissionDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGroupedByModule()
        {
            var result = await _permissionService.GetGroupedByModuleAsync();
            return Ok(result);
        }

        /// <summary>
        /// ایجاد مجوز جدید
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(PermissionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreatePermissionDto dto)
        {
            var result = await _permissionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// به‌روزرسانی اطلاعات مجوز
        /// </summary>
        [HttpPut("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(long id, [FromBody] UpdatePermissionDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "شناسه در مسیر و بدنه درخواست یکسان نیست." });

            await _permissionService.UpdateAsync(dto);
            return Ok(new { message = "مجوز با موفقیت به‌روزرسانی شد." });
        }

        /// <summary>
        /// حذف منطقی مجوز
        /// </summary>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            await _permissionService.DeleteAsync(id);
            return Ok(new { message = "مجوز با موفقیت حذف شد." });
        }
    }
}