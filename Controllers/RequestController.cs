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
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var group = await requestService.Get(id);
                    return Ok(group);
                }
                else
                {
                    return BadRequest(stringLocalizer[Constants.ERR_INVALID_INPUT_DATA].Value);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = stringLocalizer[Constants.ERR_UNEXPECTED_ERROR].Value });
            }
        }

        [HttpGet]
        [Authorize]
        [Route("list")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllRequest input)
        {
            try
            {
                return (IActionResult)await requestService.GetAll(input.Keyword, input.PageNumber, input.PageSize, input.RequestType, input.IsReviewedIncluded);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = stringLocalizer[Constants.ERR_UNEXPECTED_ERROR].Value });    
            }
        }

        [HttpPost]
        [Authorize]
        [Route("submit-group")]
        public async Task<IActionResult> Submit([FromBody] GroupRequestData input)
        {
            try
            {
                var request = await requestService.Add(input);
                return Ok(request);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = stringLocalizer[Constants.ERR_UNEXPECTED_ERROR].Value });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("submit-member")]
        public async Task<IActionResult> Submit([FromBody] MemberRequestData input)
        {
            try
            {
                var request = await requestService.Add(input);
                return Ok(request);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = stringLocalizer[Constants.ERR_UNEXPECTED_ERROR].Value });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("submit-title")]
        public async Task<IActionResult> Submit([FromBody] TitleRequestData input)
        {
            try
            {
                var request = await requestService.Add(input);
                return Ok(request);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = stringLocalizer[Constants.ERR_UNEXPECTED_ERROR].Value });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("submit-chapter")]
        public async Task<IActionResult> Submit([FromBody] ChapterRequestData input)
        {
            try
            {
                var request = await requestService.Add(input);
                return Ok(request);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = stringLocalizer[Constants.ERR_UNEXPECTED_ERROR].Value });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("submit-author")]
        public async Task<IActionResult> Submit([FromBody] AuthorRequestData input)
        {
            try
            {
                var request = await requestService.Add(input);
                return Ok(request);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = stringLocalizer[Constants.ERR_UNEXPECTED_ERROR].Value });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("submit-artist")]
        public async Task<IActionResult> Submit([FromBody] ArtistRequestData input)
        {
            try
            {
                var request = await requestService.Add(input);
                return Ok(request);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = stringLocalizer[Constants.ERR_UNEXPECTED_ERROR].Value });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("review")]
        public async Task<IActionResult> Review([FromBody] ReviewRequest input)
        {
            try
            {
                var request = await requestService.Review(input.Id, input.ReviewNote, input.IsApproved);
                return Ok(request);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = stringLocalizer[Constants.ERR_UNEXPECTED_ERROR].Value });
            }
        }
    }
}