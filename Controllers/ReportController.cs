using MangaHomeService.Models.Entities;
using MangaHomeService.Models.InputModels;
using MangaHomeService.Services;
using MangaHomeService.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace MangaHomeService.Controllers
{
    [Route("api/report")]
    [ApiController]
    public class ReportController(IReportService reportService, IStringLocalizer<SharedResources> stringLocalizer) : ControllerBase
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
                    var group = await reportService.Get(id);
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
                return (IActionResult)await reportService.GetAll(input.Keyword, input.PageNumber, input.PageSize, input.RequestType, input.IsReviewedIncluded);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = stringLocalizer[Constants.ERR_UNEXPECTED_ERROR].Value });    
            }
        }

        [HttpPost]
        [Authorize]
        [Route("submit-group")]
        public async Task<IActionResult> Submit([FromBody] GroupReportData input)
        {
            try
            {
                var request = await reportService.Add(input);
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
        public async Task<IActionResult> Submit([FromBody] TitleReportData input)
        {
            try
            {
                var request = await reportService.Add(input);
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
        public async Task<IActionResult> Submit([FromBody] ChapterReportData input)
        {
            try
            {
                var request = await reportService.Add(input);
                return Ok(request);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = stringLocalizer[Constants.ERR_UNEXPECTED_ERROR].Value });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("submit-user")]
        public async Task<IActionResult> Submit([FromBody] UserReportData input)
        {
            try
            {
                var request = await reportService.Add(input);
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
        public async Task<IActionResult> Review([FromBody] ReviewReport input)
        {
            try
            {
                var request = await reportService.Review(input.Id);
                return Ok(request);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = stringLocalizer[Constants.ERR_UNEXPECTED_ERROR].Value });
            }
        }
    }
}