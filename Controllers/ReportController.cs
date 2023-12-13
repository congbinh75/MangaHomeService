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
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IStringLocalizer<ReportController> _stringLocalizer;

        public ReportController(IReportService reportService, IStringLocalizer<ReportController> stringLocalizer)
        {
            _reportService = reportService;
            _stringLocalizer = stringLocalizer;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var group = await _reportService.Get(id);
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
                return (IActionResult)await _reportService.GetAll(input.Keyword, input.PageNumber, input.PageSize, input.RequestType, input.IsReviewedIncluded);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });    
            }
        }

        [HttpPost]
        [Authorize]
        [Route("submit-group")]
        public async Task<IActionResult> Submit([FromBody] GroupReportData input)
        {
            try
            {
                var request = await _reportService.Add(input);
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
        public async Task<IActionResult> Submit([FromBody] TitleReportData input)
        {
            try
            {
                var request = await _reportService.Add(input);
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
        public async Task<IActionResult> Submit([FromBody] ChapterReportData input)
        {
            try
            {
                var request = await _reportService.Add(input);
                return Ok(request);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("submit-user")]
        public async Task<IActionResult> Submit([FromBody] UserReportData input)
        {
            try
            {
                var request = await _reportService.Add(input);
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
        public async Task<IActionResult> Review([FromBody] ReviewReport input)
        {
            try
            {
                var request = await _reportService.Review(input.Id);
                return Ok(request);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }
    }
}