using MangaHomeService.Models;
using MangaHomeService.Models.FormDatas;
using MangaHomeService.Services;
using MangaHomeService.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace MangaHomeService.Controllers
{
    [Route("api/[controller]/[action]")]
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
        public async Task<IActionResult> Get(string id)
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetByTitle(string titleId)
        {
            try
            {
                if (!string.IsNullOrEmpty(titleId.Trim()))
                {
                    var chapters = await _chapterService.GetByTitle(titleId);
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Moderator, User")]
        public async Task<IActionResult> Create(CreateChapter input)
        {
            try
            {
                if (!int.TryParse(input.Number?.Trim(), out int number) || !string.IsNullOrEmpty(input.TitleId?.Trim())
                    || !string.IsNullOrEmpty(input.GroupId?.Trim()))
                {
                    var chapter = await _chapterService.Add(number, input.TitleId, input.GroupId, input.VolumeId, input.LanguageId);
                    return Ok(chapter);
                }
                else
                {
                    return BadRequest(_stringLocalizer["ERR_INVALID_INPUT_DATA"]);
                }
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
                    return BadRequest(ex.Message);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Moderator, User")]
        public async Task<IActionResult> Update(UpdateChapter input)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Moderator, User")]
        public async Task<IActionResult> Remove(string id)
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Moderator, User")]
        public async Task<IActionResult> UploadPage(UploadPage input)
        {
            try
            {
                if (!int.TryParse(input.Number?.Trim(), out int number)
                || !string.IsNullOrEmpty(input.ChapterId?.Trim())
                || input.File != null)
                {
                    if (4 * (input.File.Length / 3) > Constants.PageBytesLimit)
                    {
                        // TO BE FIXED
                        return BadRequest("File size exceeded 10MB limit");
                    }
                    var page = await _pageService.Add(chapterId: input.ChapterId.Trim(), number: number, file: input.File);
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Moderator, User")]
        public async Task<IActionResult> UpdatePage(UpdatePage input)
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Moderator, User")]
        public async Task<IActionResult> RemovePage(string id)
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}



