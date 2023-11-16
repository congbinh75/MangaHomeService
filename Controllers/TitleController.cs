using MangaHomeService.Models;
using MangaHomeService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MangaHomeService.Utils;
using MangaHomeService.Models.FormData;

namespace MangaHomeService.Controllers
{
    [Route("api/[controller]/[action]")]
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
        public async Task<IActionResult> GetTitlesByGenre(GetTitlesByGenreFormData getTitlesByGenreFormData)
        {
            try
            {
                int pageNumber = 0;
                if (!int.TryParse(getTitlesByGenreFormData.PageNumber, out pageNumber))
                {
                    return BadRequest();
                }

                int pageSize = 0;
                if (!int.TryParse(getTitlesByGenreFormData.PageSize, out pageSize))
                {
                    return BadRequest();
                }

                if (!string.IsNullOrEmpty(getTitlesByGenreFormData.GenreId.Trim()))
                {
                    var titles = await _titleService.AdvancedSearch(genreIds: new List<string>() { getTitlesByGenreFormData.GenreId.Trim() }, 
                        pageNumber: pageNumber, pageSize: pageSize);
                    return Ok(titles);
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
        public async Task<IActionResult> GetTitlesByTheme(GetTitlesByThemeFormData getTitlesByThemeFormData)
        {
            try
            {
                int pageNumber = 0;
                if (!int.TryParse(getTitlesByThemeFormData.PageNumber, out pageNumber))
                {
                    return BadRequest();
                }

                int pageSize = 0;
                if (!int.TryParse(getTitlesByThemeFormData.PageSize, out pageSize))
                {
                    return BadRequest();
                }

                if (!string.IsNullOrEmpty(getTitlesByThemeFormData.ThemeId.Trim()))
                {
                    var titles = await _titleService.AdvancedSearch(themeIds: new List<string>() { getTitlesByThemeFormData.ThemeId.Trim() }, 
                        pageNumber: pageNumber, pageSize: pageSize);
                    return Ok(titles);
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
        public async Task<IActionResult> Search(TitleSearchFormData searchFormData)
        {
            try
            {
                int pageNumber = 0;
                if (!int.TryParse(searchFormData.PageNumber, out pageNumber)) 
                {
                    return BadRequest(); 
                }

                int pageSize = 0;
                if (!int.TryParse (searchFormData.PageSize, out pageSize)) 
                {
                    return BadRequest();
                }

                var titles = await _titleService.Search(keyword: searchFormData.Keyword.Trim(), pageNumber: pageNumber, pageSize: pageSize);
                return Ok(titles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> AdvancedSearch(AdvancedTitleSearchFormData advancedSearchFormData)
        {
            try
            {
                int status = 0;
                List<int> statuses = new List<int>();
                if (advancedSearchFormData.Statuses != null)
                {
                    foreach (string num in advancedSearchFormData.Statuses)
                    {
                        if (!int.TryParse((string)num, out status))
                        {
                            return BadRequest();
                        }
                        statuses.Add(status);
                    }
                }

                var titles = await _titleService.AdvancedSearch(name: advancedSearchFormData.Name.Trim(), 
                    author: advancedSearchFormData.Author.Trim(), 
                    artist: advancedSearchFormData.Artist.Trim(), 
                    genreIds: advancedSearchFormData.GenreIds?.Select(x => x.Trim()).ToList(), 
                    themeIds: advancedSearchFormData.ThemeIds?.Select(x => x.Trim()).ToList(), 
                    languageIds: advancedSearchFormData.LanguageIds?.Select(x => x.Trim()).ToList(), 
                    statuses: statuses, 
                    sortByLastest: advancedSearchFormData.SortByLastest, 
                    sortByHottest: advancedSearchFormData.SortByHottest, 
                    pageNumber: advancedSearchFormData.PageNumber, 
                    pageSize: advancedSearchFormData.PageSize);
                return Ok(titles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
