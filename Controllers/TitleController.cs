using MangaHomeService.Models.InputModels;
using MangaHomeService.Services;
using MangaHomeService.Utils;
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
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;
        private readonly ITitleService _titleService;

        public TitleController(
            IConfiguration configuration,
            IStringLocalizer<SharedResources> stringLocalizer,
            ITitleService titleService)
        {
            _configuration = configuration;
            _stringLocalizer = stringLocalizer;
            _titleService = titleService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get")]
        public async Task<IActionResult> Get([FromQuery] string id)
        {
            try
            {
                var title = await _titleService.Get(id);
                return Ok(title);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"].Value });
            }
        }

        [HttpGet]
        [AllowAnonymous]
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
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"].Value });
            }
        }

        [HttpGet]
        [AllowAnonymous]
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
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"].Value });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] CreateTitle input)
        {
            try
            {
                var title = await _titleService.Add(name: input.Name, description: input.Description, artwork: input.Artwork, 
                    authorsIds: input.AuthorsIds, artistsIds: input.ArtistsIds, status: (Enums.TitleStatus)input.Status, 
                    otherNamesIds: input.OtherNamesIds, originalLanguageId: input.OriginalLanguageId, genresIds: input.GernesIds, 
                    themesIds: input.ThemesIds, demographicsIds: input.DemographicsIds);
                return Ok(title);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"].Value });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] UpdateTitle input)
        {
            try
            {
                var title = await _titleService.Update(id: input.Id, name: input.Name, description: input.Description, 
                    artwork: input.Artwork, authorsIds: input.AuthorsIds, artistsIds: input.ArtistsIds, 
                    status: (Enums.TitleStatus)input.Status, otherNamesIds: input.OtherNamesIds, 
                    originalLanguageId: input.OriginalLanguageId, genresIds: input.GernesIds, themesIds: input.ThemesIds, 
                    demographicsIds: input.DemographicsIds);
                return Ok(title);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"].Value });
            }
        }

        [HttpPost]
        [Authorize(Roles="Admin")]
        [Route("remove")]
        public async Task<IActionResult> Remove([FromBody] RemoveTitle input)
        {
            try
            {
                var title = await _titleService.Remove(input.Id);
                return Ok(title);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"].Value });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("add-rating")]
        public async Task<IActionResult> AddRating([FromBody] AddRatingTitle input)
        {
            try
            {
                var title = await _titleService.AddRating(input.Id, input.Rating, input.UserId);
                return Ok(title);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"].Value });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("update-rating")]
        public async Task<IActionResult> UpdateRating([FromBody] AddRatingTitle input)
        {
            try
            {
                var title = await _titleService.UpdateRating(input.Id, input.Rating, input.UserId);
                return Ok(title);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"].Value });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("remove-rating")]
        public async Task<IActionResult> RemoveRating([FromBody] RemoveRatingTitle input)
        {
            try
            {
                var title = await _titleService.RemoveRating(input.Id, input.UserId);
                return Ok(title);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = _stringLocalizer["ERR_UNEXPECTED_ERROR"].Value });
            }
        }
    }
}
