using MangaHomeService.Models.InputModels;
using MangaHomeService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace MangaHomeService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly IStringLocalizer<GroupController> _stringLocalizer;

        public GroupController(IGroupService groupService, IStringLocalizer<GroupController> stringLocalizer)
        {
            _groupService = groupService;
            _stringLocalizer = stringLocalizer;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var group = await _groupService.Get(id);
                    return Ok(group);
                }
                else
                {
                    return BadRequest(_stringLocalizer["ERR_INVALID_INPUT_DATA"]);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(string pageSize, string pageNumber)
        {
            try
            {
                if (int.TryParse(pageSize, out int pSize) && int.TryParse(pageNumber, out int pNumber))
                {
                    var group = await _groupService.GetAll(pSize, pNumber);
                    return Ok(group);
                }
                else
                {
                    return BadRequest(_stringLocalizer["ERR_INVALID_INPUT_DATA"]);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateGroup input)
        {
            try
            {
                var group = await _groupService.Add(input.Name, input.Description, input.ProfilePicture, input.MembersIds);
                return Ok(group);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(UpdateGroup input)
        {
            try
            {
                var group = await _groupService.Update(input.Id, input.Name, input.Description, input.ProfilePicture, input.MembersIds);
                return Ok(group);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Remove(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var group = await _groupService.Remove(id);
                    return Ok();
                }
                else
                {
                    return BadRequest(_stringLocalizer["ERR_INVALID_INPUT_DATA"]);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }
    }
}
