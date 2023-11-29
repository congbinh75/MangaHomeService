using MangaHomeService.Models.FormDatas;
using MangaHomeService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace MangaHomeService.Controllers
{
    public class GroupController : Controller
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateGroup input)
        {
            try
            {
                input.Validate();
                var group = await _groupService.Add(input.Name, input.Description, input.ProfilePicture, input.MembersIds);
                return Ok(group);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(UpdateGroup input)
        {
            try
            {

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
