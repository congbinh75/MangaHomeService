using MangaHomeService.Models.InputModels;
using MangaHomeService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace MangaHomeService.Controllers
{
    [Route("api/group")]
    [ApiController]
    public class GroupController(IGroupService groupService) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("get")]
        public async Task<IActionResult> Get([FromQuery] GetGroup input)
        {
            var group = await groupService.Get(input.Id);
            return Ok(group);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("list")]
        public async Task<IActionResult> GetAll([FromQuery] int pageSize, int pageNumber)
        {
            var group = await groupService.GetAll(pageSize, pageNumber);
            return Ok(group);
        }

        [HttpPost]
        [Authorize]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] CreateGroup input)
        {
            var group = await groupService.Add(input.Name, input.Description, input.ProfilePicture, input.MembersIds);
            return Ok(group);
        }

        [HttpPost]
        [Authorize]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] UpdateGroup input)
        {
            var group = await groupService.Update(input.Id, input.Name, input.Description, input.ProfilePicture, input.MembersIds);
            return Ok(group);
        }

        [HttpPost]
        [Authorize]
        [Route("remove")]
        public async Task<IActionResult> Remove([FromBody] RemoveGroup input)
        {
            await groupService.Remove(input.GroupId);
            return Ok();
        }
    }
}
