using MangaHomeService.Models.InputModels;
using MangaHomeService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MangaHomeService.Controllers
{
    [Route("api/report")]
    [ApiController]
    public class ReportController(IReportService reportService) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        [Route("get")]
        public async Task<IActionResult> Get([FromQuery] string id)
        {
            var group = await reportService.Get(id);
            return Ok(group);
        }

        [HttpGet]
        [Authorize]
        [Route("list")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllRequest input)
        {
            return (IActionResult)await reportService.GetAll(input.Keyword, input.PageNumber, input.PageSize, input.RequestType, input.IsReviewedIncluded);
        }

        [HttpPost]
        [Authorize]
        [Route("submit-group")]
        public async Task<IActionResult> Submit([FromBody] GroupReportData input)
        {
            var request = await reportService.Add(input);
            return Ok(request);
        }

        [HttpPost]
        [Authorize]
        [Route("submit-title")]
        public async Task<IActionResult> Submit([FromBody] TitleReportData input)
        {
            var request = await reportService.Add(input);
            return Ok(request);
        }

        [HttpPost]
        [Authorize]
        [Route("submit-chapter")]
        public async Task<IActionResult> Submit([FromBody] ChapterReportData input)
        {
            var request = await reportService.Add(input);
            return Ok(request);
        }

        [HttpPost]
        [Authorize]
        [Route("submit-user")]
        public async Task<IActionResult> Submit([FromBody] UserReportData input)
        {
            var request = await reportService.Add(input);
            return Ok(request);
        }

        [HttpPost]
        [Authorize]
        [Route("review")]
        public async Task<IActionResult> Review([FromBody] ReviewReport input)
        {
            var request = await reportService.Review(input.Id);
            return Ok(request);
        }
    }
}