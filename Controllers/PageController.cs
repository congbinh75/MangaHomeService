using MangaHomeService.Models.Entities;
using MangaHomeService.Models.InputModels;
using MangaHomeService.Services;
using MangaHomeService.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace MangaHomeService.Controllers
{
    [Route("api/page")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private IConfiguration _configuration;
        private IStringLocalizer<ChapterController> _stringLocalizer;
        private IPageService _pageService;

        public PageController(
            IConfiguration configuration,
            IStringLocalizer<ChapterController> stringLocalizer,
            IPageService pageService)
        {
            _configuration = configuration;
            _stringLocalizer = stringLocalizer;
            _pageService = pageService;
        }

        [HttpPost]
        [Authorize]
        [Route("/{id}")]
        public async Task<IActionResult> Get([FromBody] UploadPage input)
        {
            try
            {
                var page = await _pageService.Add(chapterId: input.ChapterId.Trim(), number: input.Number, file: input.File);
                return Ok(page);
            }
            catch (NotFoundException)
            {
                return BadRequest(_stringLocalizer["ERR_CHAPTER_NOT_FOUND"]);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("upload")]
        public async Task<IActionResult> Upload([FromBody] UploadPage input)
        {
            try
            {
                if (4 * (input.File.Length / 3) > Constants.PageBytesLimit)
                {
                    // TO BE FIXED
                    return BadRequest("File size exceeded 10MB limit");
                }
                var page = await _pageService.Add(chapterId: input.ChapterId.Trim(), number: input.Number, file: input.File);
                return Ok(page);
            }
            catch (NotFoundException)
            {
                return BadRequest(_stringLocalizer["ERR_CHAPTER_NOT_FOUND"]);
            }
            catch (ArgumentException)
            {
                //TO BE FIXED
                return BadRequest(_stringLocalizer[""]);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] UpdatePage input)
        {
            try
            {
                var page = await _pageService.Update(id: input.Id.Trim(), number: input.Number);
                return Ok(page);
            }
            catch (NotFoundException)
            {
                return BadRequest(_stringLocalizer["ERR_CHAPTER_NOT_FOUND"]);
            }
            catch (ArgumentException)
            {
                //TO BE FIXED
                return BadRequest(_stringLocalizer[""]);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("remove")]
        public async Task<IActionResult> Remove([FromBody] string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id.Trim()))
                {
                    await _pageService.Remove(id.Trim());
                    return Ok();
                }
                else
                {
                    return BadRequest(_stringLocalizer["ERR_INVALID_INPUT_DATA"]);
                }
            }
            catch (NotFoundException)
            {
                return BadRequest(_stringLocalizer["ERR_PAGE_NOT_FOUND"]);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }
    }
}
