using MangaHomeService.Models.Entities;
using MangaHomeService.Models.InputModels;
using MangaHomeService.Services;
using MangaHomeService.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace MangaHomeService.Controllers
{
    [Route("api/chapter")]
    [ApiController]
    public class ChapterController : ControllerBase
    {
        private IConfiguration _configuration;
        private IStringLocalizer<ChapterController> _stringLocalizer;
        private ITitleService _titleService;
        private IChapterService _chapterService;
        private IPageService _pageService;

        public ChapterController(
            IConfiguration configuration,
            IStringLocalizer<ChapterController> stringLocalizer,
            ITitleService titleService,
            IChapterService chapterService,
            IPageService pageService)
        {
            _configuration = configuration;
            _stringLocalizer = stringLocalizer;
            _titleService = titleService;
            _chapterService = chapterService;
            _pageService = pageService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id.Trim()))
                {
                    var chapter = await _chapterService.Get(id);
                    return Ok(chapter);
                }
                else
                {
                    return BadRequest(_stringLocalizer["ERR_INVALID_INPUT_DATA"]);
                }
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetByTitle([FromQuery] string titleId, int pageNumber, int pageSize)
        {
            try
            {
                if (!string.IsNullOrEmpty(titleId.Trim()))
                {
                    var chapters = await _chapterService.GetByTitle(titleId, pageNumber, pageSize);
                    return Ok(chapters);
                }
                else
                {
                    return BadRequest(_stringLocalizer["ERR_INVALID_INPUT_DATA"]);
                }
            }
            catch (NotFoundException)
            {
                return BadRequest(_stringLocalizer["ERR_TITLE_NOT_FOUND"]);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Moderator, User")]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] CreateChapter input)
        {
            try
            {
                var chapter = await _chapterService.Add(input.Number, input.TitleId, input.GroupId, input.VolumeId, input.LanguageId);
                return Ok(chapter);
            }
            catch (NotFoundException ex)
            {
                if (ex.Message == nameof(Title))
                {
                    return BadRequest(_stringLocalizer["ERR_TITLE_NOT_FOUND"]);
                }
                else if (ex.Message == nameof(Group))
                {
                    return BadRequest(_stringLocalizer["ERR_GROUP_NOT_FOUND"]);
                }
                else if (ex.Message == nameof(Volume))
                {
                    return BadRequest(_stringLocalizer["ERR_VOLUME_NOT_FOUND"]);
                }
                else if (ex.Message == nameof(Language))
                {
                    return BadRequest(_stringLocalizer["ERR_LANGUAGE_NOT_FOUND"]);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Moderator, User")]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] UpdateChapter input)
        {
            try
            {
                var chapter = await _chapterService.Update(input.Id, input.Number, input.TitleId, input.GroupId, input.VolumeId, input.LanguageId);
                return Ok(chapter);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Moderator, User")]
        [Route("remove")]
        public async Task<IActionResult> Remove([FromBody] string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    await _chapterService.Remove(id);
                    return Ok();
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

        [HttpPost]
        [Authorize(Roles = "Admin, Moderator, User")]
        [Route("upload-page")]
        public async Task<IActionResult> UploadPage([FromBody] UploadPage input)
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
        [Authorize(Roles = "Admin, Moderator, User")]
        [Route("update-page")]
        public async Task<IActionResult> UpdatePage([FromBody] UpdatePage input)
        {
            try
            {
                if (!string.IsNullOrEmpty(input.Id?.Trim()) && int.TryParse(input.Number.Trim(), out int number))
                {
                    var page = await _pageService.Update(id: input.Id.Trim(), number: number);
                    return Ok(page);
                }
                else
                {
                    return BadRequest(_stringLocalizer["ERR_INVALID_INPUT_DATA"]);
                }
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
        [Route("remove-page")]
        public async Task<IActionResult> RemovePage([FromBody] string id)
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



