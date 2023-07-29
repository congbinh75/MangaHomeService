using MangaHomeService.Models;
using MangaHomeService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MangaHomeService.Utils;

namespace MangaHomeService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitleController : ControllerBase
    {
        private IConfiguration _configuration;
        private IStringLocalizer<UserController> _stringLocalizer;
        private ITitleService _titleService;

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
                    return BadRequest(_stringLocalizer["ERR_MISSING_INPUT_DATA"]);
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var hottestTitles = _titleService.Search(sortByHottest: true);
                var lastestTitles = _titleService.Search(sortByLastest: true);

                return Ok(new { hottestTitles, lastestTitles });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTitlesByGenre(string genreId, int count, int page)
        {
            try
            {
                if (!string.IsNullOrEmpty(genreId.Trim()))
                {
                    var titles = _titleService.Search(genreIds: new List<string>() { genreId.Trim() }, count: count, page: page);
                    return Ok(titles);
                }
                else
                {
                    return BadRequest(_stringLocalizer["ERR_MISSING_INPUT_DATA"]);
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTitlesByTheme(string themeId, int count, int page)
        {
            try
            {
                if (!string.IsNullOrEmpty(themeId.Trim()))
                {
                    var titles = _titleService.Search(themeIds: new List<string>() { themeId.Trim() }, count: count, page: page);
                    return Ok(titles);
                }
                else
                {
                    return BadRequest(_stringLocalizer["ERR_MISSING_INPUT_DATA"]);
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Search(string keyword, int count, int page)
        {
            try
            {
                var titles = await _titleService.Search(keyword: keyword.Trim(), count: count, page: page);
                return Ok(titles);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Search(string name = "", string author = "", string artist = "", List<string>? genreIds = null,
            List<string>? themeIds = null, string originalLanguageId = "", List<string>? languageIds = null, List<int>? statuses = null,
            bool sortByLastest = false, bool sortByHottest = false, int count = Constants.TitlesPerPage, int page = 1)
        {
            try
            {
                var titles = await _titleService.Search(name: name.Trim(), author: author.Trim(), artist: artist.Trim(), 
                    genreIds: genreIds?.Select(x => x.Trim()).ToList(), themeIds: themeIds?.Select(x => x.Trim()).ToList(), 
                    languageIds: languageIds?.Select(x => x.Trim()).ToList(), statuses: statuses, 
                    sortByLastest: sortByLastest, sortByHottest: sortByHottest, count: count, page: page);
                return Ok(titles);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
