using MangaHomeService.Models.InputModels;
using MangaHomeService.Services;
using MangaHomeService.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace MangaHomeService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private IConfiguration _configuration;
        private IStringLocalizer<AdminController> _stringLocalizer;
        private IUserService _userService;

        public AdminController(
            IConfiguration configuration,
            IStringLocalizer<AdminController> stringLocalizer,
            IUserService userService)
        {
            _configuration = configuration;
            _stringLocalizer = stringLocalizer;
            _userService = userService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRoleOfUser(UpdateRoleOfUser input)
        {
            try
            {
                int role = -1;
                if (string.IsNullOrEmpty(input.UserId) || string.IsNullOrEmpty(input.Role)
                    || !int.TryParse(input.Role, out role) || !Enum.IsDefined(typeof(Enums.Role), role))
                {
                    return BadRequest(_stringLocalizer["ERR_INVALID_INPUT_DATA"]);
                }
                var user = await _userService.Update(input.UserId, role: role);
                return Ok(user);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddChapterLangauage(AddChapterLanguage input)
        {
            throw new NotImplementedException();
        }
    }
}
