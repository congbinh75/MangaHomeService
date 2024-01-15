using MangaHomeService.Models.InputModels;
using MangaHomeService.Services;
using MangaHomeService.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Localization;

namespace MangaHomeService.Controllers
{
    [Route("api/page")]
    [ApiController]
    public class PageController(IStringLocalizer<SharedResources> stringLocalizer, IPageService pageService) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("get")]
        public async Task<IActionResult> Get([FromQuery] GetPage input)
        {
            try
            {
                var page = await pageService.Get(input.Id);
                return Ok(page);
            }
            catch (NotFoundException)
            {
                return NotFound(stringLocalizer["ERR_PAGE_NOT_FOUND"].Value);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get-file")]
        public async Task<IActionResult> GetFile([FromQuery] GetPage input)
        {
            try
            {
                var page = await pageService.Get(input.Id);
                if (!System.IO.File.Exists(page.FilePath))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = stringLocalizer[Constants.ERR_FILE_NOT_FOUND].Value });
                }
                var file = System.IO.File.ReadAllBytes(page.FilePath);
                var provider = new FileExtensionContentTypeProvider();
                const string DefaultContentType = "application/octet-stream";
                if (!provider.TryGetContentType(page.FilePath, out string? contentType))
                {
                    contentType = DefaultContentType;
                }
                return File(file, contentType);
            }
            catch (NotFoundException)
            {
                return NotFound(stringLocalizer[Constants.ERR_PAGE_NOT_FOUND].Value);
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
                var page = await pageService.Add(chapterId: input.ChapterId.Trim(), number: input.Number, file: input.File);
                return Ok(page);
            }
            catch (NotFoundException)
            {
                return NotFound(stringLocalizer[Constants.ERR_CHAPTER_NOT_FOUND].Value);
            }
            catch (ArgumentException)
            {
                //TO BE FIXED
                return BadRequest(stringLocalizer[""]);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] UpdatePage input)
        {
            try
            {
                var page = await pageService.Update(id: input.Id.Trim(), number: input.Number);
                return Ok(page);
            }
            catch (NotFoundException)
            {
                return NotFound(stringLocalizer[Constants.ERR_PAGE_NOT_FOUND].Value);
            }
            catch (ArgumentException)
            {
                //TO BE FIXED
                return BadRequest(stringLocalizer[""]);
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
                    await pageService.Remove(id.Trim());
                    return Ok();
                }
                else
                {
                    return BadRequest(stringLocalizer[Constants.ERR_INVALID_INPUT_DATA].Value);
                }
            }
            catch (NotFoundException)
            {
                return BadRequest(stringLocalizer[Constants.ERR_PAGE_NOT_FOUND].Value);
            }
        }
    }
}
