using MangaHomeService.Models.InputModels;
using MangaHomeService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace MangaHomeService.Controllers
{
    [Route("api/group")]
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
        [Route("get")]
        public async Task<IActionResult> Get([FromQuery] GetGroup input)
        {
            try
            {
                var group = await _groupService.Get(input.Id);
                return Ok(group);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("list")]
        public async Task<IActionResult> GetAll([FromQuery] int pageSize, int pageNumber)
        {
            try
            {
                var group = await _groupService.GetAll(pageSize, pageNumber);
                return Ok(group);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] CreateGroup input)
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
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] UpdateGroup input)
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
        [Route("remove")]
        public async Task<IActionResult> Remove([FromBody] RemoveGroup input)
        {
            try
            {
                var group = await _groupService.Remove(input.GroupId);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }
    }
}
