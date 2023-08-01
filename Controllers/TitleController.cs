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
                var hottestTitles = _titleService.AdvancedSearch(sortByHottest: true);
                var lastestTitles = _titleService.AdvancedSearch(sortByLastest: true);

                return Ok(new { hottestTitles, lastestTitles });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTitlesByGenre(GetTitlesByGenreFormData getTitlesByGenreFormData)
        {
            try
            {
                if (!string.IsNullOrEmpty(getTitlesByGenreFormData.GenreId.Trim()))
                {
                    var titles = _titleService.AdvancedSearch(genreIds: new List<string>() { getTitlesByGenreFormData.GenreId.Trim() }, 
                        pageNumber: getTitlesByGenreFormData.PageNumber, pageSize: getTitlesByGenreFormData.PageSize);
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
        public async Task<IActionResult> GetTitlesByTheme(GetTitlesByThemeFormData getTitlesByThemeFormData)
        {
            try
            {
                if (!string.IsNullOrEmpty(getTitlesByThemeFormData.ThemeId.Trim()))
                {
                    var titles = _titleService.AdvancedSearch(themeIds: new List<string>() { getTitlesByThemeFormData.ThemeId.Trim() }, 
                        pageNumber: getTitlesByThemeFormData.PageNumber, pageSize: getTitlesByThemeFormData.PageSize);
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
        public async Task<IActionResult> Search(SearchFormData searchFormData)
        {
            try
            {
                var titles = await _titleService.Search(keyword: searchFormData.Keyword.Trim(), 
                    pageNumber: searchFormData.PageNumber, pageSize: searchFormData.PageSize);
                return Ok(titles);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> AdvancedSearch(AdvancedSearchFormData advancedSearchFormData)
        {
            try
            {
                var titles = await _titleService.AdvancedSearch(name: advancedSearchFormData.Name.Trim(), 
                    author: advancedSearchFormData.Author.Trim(), 
                    artist: advancedSearchFormData.Artist.Trim(), 
                    genreIds: advancedSearchFormData.GenreIds?.Select(x => x.Trim()).ToList(), 
                    themeIds: advancedSearchFormData.ThemeIds?.Select(x => x.Trim()).ToList(), 
                    languageIds: advancedSearchFormData.LanguageIds?.Select(x => x.Trim()).ToList(), 
                    statuses: advancedSearchFormData.Statuses, 
                    sortByLastest: advancedSearchFormData.SortByLastest, 
                    sortByHottest: advancedSearchFormData.SortByHottest, 
                    pageNumber: advancedSearchFormData.PageNumber, 
                    pageSize: advancedSearchFormData.PageSize);
                return Ok(titles);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
