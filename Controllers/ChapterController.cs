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
    public class ChapterController(IStringLocalizer<SharedResources> stringLocalizer, IChapterService chapterService) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("get")]
        public async Task<IActionResult> Get([FromQuery] GetChapter input)
        {
            try
            {
                var chapter = await chapterService.Get(input.Id);
                return Ok(chapter);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = stringLocalizer[Constants.ERR_UNEXPECTED_ERROR].Value });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get-by-title")]
        public async Task<IActionResult> GetByTitle([FromQuery] GetChaptersByTitle input)
        {
            try
            {
                var chapters = await chapterService.GetByTitle(input.TitleId, input.PageNumber, input.PageSize);
                return Ok(chapters);
            }
            catch (NotFoundException)
            {
                return NotFound(stringLocalizer[Constants.ERR_TITLE_NOT_FOUND].Value);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = stringLocalizer[Constants.ERR_UNEXPECTED_ERROR].Value });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] CreateChapter input)
        {
            try
            {
                var chapter = await chapterService.Add(input.Number, input.TitleId, input.GroupId, input.VolumeId, input.LanguageId);
                return Ok(chapter);
            }
            catch (NotFoundException ex)
            {
                if (ex.Message == nameof(Title))
                {
                    return NotFound(stringLocalizer[Constants.ERR_TITLE_NOT_FOUND].Value);
                }
                else if (ex.Message == nameof(Group))
                {
                    return NotFound(stringLocalizer[Constants.ERR_GROUP_NOT_FOUND].Value);
                }
                else if (ex.Message == nameof(Volume))
                {
                    return NotFound(stringLocalizer[Constants.ERR_VOLUME_NOT_FOUND].Value);
                }
                else if (ex.Message == nameof(Language))
                {
                    return NotFound(stringLocalizer[Constants.ERR_LANGUAGE_NOT_FOUND].Value);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = stringLocalizer[Constants.ERR_UNEXPECTED_ERROR].Value });
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = stringLocalizer[Constants.ERR_UNEXPECTED_ERROR].Value });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] UpdateChapter input)
        {
            try
            {
                var chapter = await chapterService.Update(input.Id, input.Number, input.TitleId, input.GroupId, input.VolumeId, input.LanguageId);
                return Ok(chapter);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = stringLocalizer[Constants.ERR_UNEXPECTED_ERROR].Value });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("remove")]
        public async Task<IActionResult> Remove([FromBody] RemoveChapter input)
        {
            try
            {
                await chapterService.Remove(input.Id);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = stringLocalizer[Constants.ERR_UNEXPECTED_ERROR].Value });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("set-approval")]
        public async Task<IActionResult> SetApproval([FromBody] SetApprovalChapter input)
        {
            try
            {
                var chapter = await chapterService.Update(id: input.ChapterId, isApproved: input.IsApproved);
                return Ok(chapter);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = stringLocalizer[Constants.ERR_UNEXPECTED_ERROR].Value });
            }
        }
    }
}



