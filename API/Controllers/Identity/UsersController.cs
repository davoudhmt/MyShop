using API.Middlewares;
using Application.DTOs.Common;
using Application.DTOs.Identity.User;
using Application.Interfaces.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers.Identity
{
    /// <summary>
    /// کنترلر مدیریت کاربران (Users) در پنل ادمین
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // دسترسی به این کنترلر نیازمند احراز هویت است
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// سازنده کنترلر با تزریق سرویس مدیریت کاربران
        /// </summary>
        /// <param name="userService">سرویس مدیریت کاربران</param>
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// دریافت لیست کاربران با فیلتر و صفحه‌بندی
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPaged([FromQuery] UserFilterDto filter)
        {
            var result = await _userService.GetPagedAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// دریافت کاربر با شناسه
        /// </summary>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _userService.GetByIdAsync(id);
            if (result is null)
                return NotFound(new { message = $"کاربر با شناسه {id} یافت نشد." });

            return Ok(result);
        }

        /// <summary>
        /// حذف منطقی کاربر
        /// </summary>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            await _userService.DeleteAsync(id);
            return Ok(new { message = "کاربر با موفقیت حذف شد." });
        }

        /// <summary>
        /// فعال‌سازی کاربر
        /// </summary>
        [HttpPut("{id:long}/activate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Activate(long id)
        {
            await _userService.ActivateAsync(id);
            return Ok(new { message = "کاربر با موفقیت فعال شد." });
        }

        /// <summary>
        /// غیرفعال‌سازی کاربر
        /// </summary>
        [HttpPut("{id:long}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(long id)
        {
            await _userService.DeactivateAsync(id);
            return Ok(new { message = "کاربر با موفقیت غیرفعال شد." });
        }
    }
}