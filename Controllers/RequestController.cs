using MangaHomeService.Models.InputModels;
using MangaHomeService.Services;
using MangaHomeService.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace MangaHomeService.Controllers
{
    [Route("api/request")]
    [ApiController]
    public class RequestController(IRequestService requestService, IStringLocalizer<SharedResources> stringLocalizer) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        [Route("get")]
        public async Task<IActionResult> Get([FromQuery] string id)
        {
            var group = await requestService.Get(id);
            return Ok(group);
        }

        [HttpGet]
        [Authorize]
        [Route("list")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllRequest input)
        {
            return (IActionResult)await requestService.GetAll(input.Keyword, input.PageNumber, input.PageSize, input.RequestType, input.IsReviewedIncluded);
        }

        [HttpPost]
        [Authorize]
        [Route("submit-group")]
        public async Task<IActionResult> Submit([FromBody] GroupRequestData input)
        {
            var request = await requestService.Add(input);
            return Ok(request);
        }

        [HttpPost]
        [Authorize]
        [Route("submit-member")]
        public async Task<IActionResult> Submit([FromBody] MemberRequestData input)
        {
            var request = await requestService.Add(input);
            return Ok(request);
        }

        [HttpPost]
        [Authorize]
        [Route("submit-title")]
        public async Task<IActionResult> Submit([FromBody] TitleRequestData input)
        {
            var request = await requestService.Add(input);
            return Ok(request);
        }

        [HttpPost]
        [Authorize]
        [Route("submit-chapter")]
        public async Task<IActionResult> Submit([FromBody] ChapterRequestData input)
        {
            var request = await requestService.Add(input);
            return Ok(request);
        }

        [HttpPost]
        [Authorize]
        [Route("submit-author")]
        public async Task<IActionResult> Submit([FromBody] AuthorRequestData input)
        {
            var request = await requestService.Add(input);
            return Ok(request);
        }

        [HttpPost]
        [Authorize]
        [Route("submit-artist")]
        public async Task<IActionResult> Submit([FromBody] ArtistRequestData input)
        {
            var request = await requestService.Add(input);
            return Ok(request);
        }

        [HttpPost]
        [Authorize]
        [Route("review")]
        public async Task<IActionResult> Review([FromBody] ReviewRequest input)
        {
            var request = await requestService.Review(input.Id, input.ReviewNote, input.IsApproved);
            return Ok(request);
        }
    }
}