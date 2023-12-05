using MangaHomeService.Models.InputModels;
using MangaHomeService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace MangaHomeService.Controllers
{
    [Route("api/title")]
    [ApiController]
    public class TitleController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<TitleController> _stringLocalizer;
        private readonly ITitleService _titleService;

        public TitleController(
            IConfiguration configuration,
            IStringLocalizer<TitleController> stringLocalizer,
            ITitleService titleService)
        {
            _configuration = configuration;
            _stringLocalizer = stringLocalizer;
            _titleService = titleService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string id)
        {
            try
            {
                var title = await _titleService.Get(id);
                return Ok(title);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> Search([FromQuery] TitleSearch input)
        {
            try
            {
                var titles = await _titleService.Search(keyword: input.Keyword, pageNumber: input.PageNumber, pageSize: input.PageSize);
                return Ok(titles);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("advanced-search")]
        public async Task<IActionResult> AdvancedSearch([FromQuery] AdvancedTitleSearch input)
        {
            try
            {
                int status = 0;
                List<int> statuses = [];
                if (input.Statuses != null)
                {
                    foreach (string num in input.Statuses)
                    {
                        if (!int.TryParse(num, out status))
                        {
                            return BadRequest();
                        }
                        statuses.Add(status);
                    }
                }

                var titles = await _titleService.AdvancedSearch(name: input.Name,
                    author: input.Author,
                    artist: input.Artist,
                    genreIds: input.GenreIds?.Select(x => x.Trim()).ToList(),
                    themeIds: input.ThemeIds?.Select(x => x.Trim()).ToList(),
                    languageIds: input.LanguageIds?.Select(x => x.Trim()).ToList(),
                    statuses: statuses,
                    sortByLastest: input.SortByLastest,
                    sortByHottest: input.SortByHottest,
                    pageNumber: input.PageNumber,
                    pageSize: input.PageSize);
                return Ok(titles);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"] });
            }
        }
    }
}
