using MangaHomeService.Models;
using MangaHomeService.Models.FormData;
using MangaHomeService.Services.Interfaces;
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
        private IStringLocalizer<UserController> _stringLocalizer;
        private ITitleService _titleService;
        private IChapterService _chapterService;
        private IPageService _pageService;
        
        public ChapterController(
            IConfiguration configuration,
            IStringLocalizer<UserController> stringLocalizer,
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

        [HttpPost]
        [Authorize(Roles = "Admin, Moderator, User")]
        public async Task<IActionResult> Add(AddChapterFormData formData)
        {
            try
            {
                int number = 0;
                if (!int.TryParse(formData.Number.Trim(), out number)
                    || !string.IsNullOrEmpty(formData.TitleId.Trim()) 
                    || !string.IsNullOrEmpty(formData.GroupId.Trim()))
                {
                    var chapter = await _chapterService.Add(number, formData.TitleId, formData.GroupId, formData.VolumeId, formData.LanguageId);
                    return Ok(chapter);
                }
                else
                {
                    return BadRequest(_stringLocalizer["ERR_INVALID_INPUT_DATA"]);
                }
            }
            catch (NotFoundException ex)
            {
                if (ex.Message == typeof(Title).ToString())
                    return BadRequest(_stringLocalizer["ERR_TITLE_NOT_FOUND"]);
                else if (ex.Message == typeof(Group).ToString())
                    return BadRequest(_stringLocalizer["ERR_GROUP_NOT_FOUND"]);
                else if (ex.Message == typeof(Volume).ToString())
                    return BadRequest(_stringLocalizer["ERR_VOLUME_NOT_FOUND"]);
                else if (ex.Message == typeof(Language).ToString())
                    return BadRequest(_stringLocalizer["ERR_LANGUAGE_NOT_FOUND"]);
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
