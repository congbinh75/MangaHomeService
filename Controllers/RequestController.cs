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
    public class RequestController : ControllerBase
    {
        private readonly IRequestService _requestService;
        private readonly IStringLocalizer<RequestController> _stringLocalizer;

        public RequestController(IRequestService requestService, IStringLocalizer<RequestController> stringLocalizer)
        {
            _requestService = requestService;
            _stringLocalizer = stringLocalizer;
        }

        [HttpGet]
        [Authorize]
        [Route("get")]
        public async Task<IActionResult> Get([FromQuery] string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var group = await _requestService.Get(id);
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
        [Authorize]
        [Route("list")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllRequest input)
        {
            try
            {
                return (IActionResult)await _requestService.GetAll(input.Keyword, input.PageNumber, input.PageSize, input.RequestType, input.IsReviewedIncluded);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });    
            }
        }

        [HttpPost]
        [Authorize]
        [Route("submit-group")]
        public async Task<IActionResult> Submit([FromBody] GroupRequestData input)
        {
            try
            {
                var request = await _requestService.Add(input);
                return Ok(request);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("submit-member")]
        public async Task<IActionResult> Submit([FromBody] MemberRequestData input)
        {
            try
            {
                var request = await _requestService.Add(input);
                return Ok(request);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("submit-title")]
        public async Task<IActionResult> Submit([FromBody] TitleRequestData input)
        {
            try
            {
                var request = await _requestService.Add(input);
                return Ok(request);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("submit-chapter")]
        public async Task<IActionResult> Submit([FromBody] ChapterRequestData input)
        {
            try
            {
                var request = await _requestService.Add(input);
                return Ok(request);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("submit-author")]
        public async Task<IActionResult> Submit([FromBody] AuthorRequestData input)
        {
            try
            {
                var request = await _requestService.Add(input);
                return Ok(request);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("submit-artist")]
        public async Task<IActionResult> Submit([FromBody] ArtistRequestData input)
        {
            try
            {
                var request = await _requestService.Add(input);
                return Ok(request);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("review")]
        public async Task<IActionResult> Review([FromBody] ReviewRequest input)
        {
            try
            {
                var request = await _requestService.Review(input.Id, input.ReviewNote, input.IsApproved);
                return Ok(request);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }
    }
}