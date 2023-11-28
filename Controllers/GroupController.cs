using MangaHomeService.Services;
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

        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var group = await _groupService.Get(id);
                return Ok(group);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

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

    }
}
