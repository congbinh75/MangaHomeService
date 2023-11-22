using MangaHomeService.Models.FormDatas.Title;
using MangaHomeService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace MangaHomeService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TitleController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<UserController> _stringLocalizer;
        private readonly ITitleService _titleService;

        public TitleController(
            IConfiguration configuration,
            IStringLocalizer<UserController> stringLocalizer,
            ITitleService titleService)
        {
            _configuration = configuration;
            _stringLocalizer = stringLocalizer;
            _titleService = titleService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id.Trim()))
                {
                    var title = await _titleService.Get(id);
                    return Ok(title);
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

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var hottestTitles = await _titleService.AdvancedSearch(sortByHottest: true);
                var lastestTitles = await _titleService.AdvancedSearch(sortByLastest: true);

                return Ok(new { hottestTitles, lastestTitles });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Search(Search input)
        {
            try
            {
                int pageNumber = 0;
                if (!int.TryParse(input.PageNumber, out pageNumber))
                {
                    return BadRequest();
                }

                int pageSize = 0;
                if (!int.TryParse(input.PageSize, out pageSize))
                {
                    return BadRequest();
                }

                var titles = await _titleService.Search(keyword: input.Keyword.Trim(), pageNumber: pageNumber, pageSize: pageSize);
                return Ok(titles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> AdvancedSearch(AdvancedSearch input)
        {
            try
            {
                int status = 0;
                List<int> statuses = new List<int>();
                if (input.Statuses != null)
                {
                    foreach (string num in input.Statuses)
                    {
                        if (!int.TryParse((string)num, out status))
                        {
                            return BadRequest();
                        }
                        statuses.Add(status);
                    }
                }

                var titles = await _titleService.AdvancedSearch(name: input.Name.Trim(),
                    author: input.Author.Trim(),
                    artist: input.Artist.Trim(),
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
