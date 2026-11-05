using API.Middlewares;
using Application.DTOs.Common;
using Application.DTOs.Identity.Role;
using Application.Interfaces.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers.Identity
{
    /// <summary>
    /// کنترلر مدیریت نقش‌ها (Roles)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // دسترسی به این کنترلر نیازمند احراز هویت است
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        /// <summary>
        /// سازنده کنترلر با تزریق سرویس مدیریت نقش‌ها
        /// </summary>
        /// <param name="roleService">سرویس مدیریت نقش‌ها</param>
        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// دریافت لیست نقش‌ها با فیلتر و صفحه‌بندی
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<RoleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPaged([FromQuery] RoleFilterDto filter)
        {
            var result = await _roleService.GetPagedAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// دریافت نقش با شناسه (خلاصه)
        /// </summary>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _roleService.GetByIdAsync(id);
            if (result is null)
                return NotFound(new { message = $"نقش با شناسه {id} یافت نشد." });

            return Ok(result);
        }

        /// <summary>
        /// دریافت جزئیات کامل نقش به همراه لیست مجوزهای تخصیص‌یافته
        /// </summary>
        [HttpGet("{id:long}/details")]
        [ProducesResponseType(typeof(RoleDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDetails(long id)
        {
            var result = await _roleService.GetDetailsAsync(id);
            if (result is null)
                return NotFound(new { message = $"نقش با شناسه {id} یافت نشد." });

            return Ok(result);
        }

        /// <summary>
        /// ایجاد نقش جدید
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(RoleDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateRoleDto dto)
        {
            var result = await _roleService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// به‌روزرسانی اطلاعات نقش
        /// </summary>
        [HttpPut("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateRoleDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "شناسه در مسیر و بدنه درخواست یکسان نیست." });

            await _roleService.UpdateAsync(dto);
            return Ok(new { message = "نقش با موفقیت به‌روزرسانی شد." });
        }

        /// <summary>
        /// حذف منطقی نقش
        /// </summary>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            await _roleService.DeleteAsync(id);
            return Ok(new { message = "نقش با موفقیت حذف شد." });
        }
    }
}