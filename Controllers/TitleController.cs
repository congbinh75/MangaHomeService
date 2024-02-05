using MangaHomeService.Models.InputModels;
using MangaHomeService.Services;
using MangaHomeService.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MangaHomeService.Controllers
{
    [Route("api/title")]
    [ApiController]
    public class TitleController(ITitleService titleService) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("get")]
        public async Task<IActionResult> Get([FromQuery] string id)
        {
            var title = await titleService.Get(id);
            return Ok(title);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("search")]
        public async Task<IActionResult> Search([FromQuery] TitleSearch input)
        {
            var titles = await titleService.Search(keyword: input.Keyword, pageNumber: input.PageNumber, pageSize: input.PageSize);
            return Ok(titles);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("advanced-search")]
        public async Task<IActionResult> AdvancedSearch([FromQuery] AdvancedTitleSearch input)
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

            var titles = await titleService.AdvancedSearch(name: input.Name,
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

        [HttpPost]
        [Authorize]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] CreateTitle input)
        {
            var title = await titleService.Add(name: input.Name, description: input.Description, artwork: input.Artwork,
                authorsIds: input.AuthorsIds, artistsIds: input.ArtistsIds, status: (Enums.TitleStatus)input.Status,
                otherNamesIds: input.OtherNamesIds, originalLanguageId: input.OriginalLanguageId, genresIds: input.GernesIds,
                themesIds: input.ThemesIds, demographicsIds: input.DemographicsIds);
            return Ok(title);
        }

        [HttpPost]
        [Authorize]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] UpdateTitle input)
        {
            var title = await titleService.Update(id: input.Id, name: input.Name, description: input.Description,
                artwork: input.Artwork, authorsIds: input.AuthorsIds, artistsIds: input.ArtistsIds,
                status: (Enums.TitleStatus)input.Status, otherNamesIds: input.OtherNamesIds,
                originalLanguageId: input.OriginalLanguageId, genresIds: input.GernesIds, themesIds: input.ThemesIds,
                demographicsIds: input.DemographicsIds);
            return Ok(title);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("remove")]
        public async Task<IActionResult> Remove([FromBody] RemoveTitle input)
        {
            var title = await titleService.Remove(input.Id);
            return Ok(title);
        }

        [HttpPost]
        [Authorize]
        [Route("add-rating")]
        public async Task<IActionResult> AddRating([FromBody] AddRatingTitle input)
        {
            var title = await titleService.AddRating(input.Id, input.Rating, input.UserId);
            return Ok(title);
        }

        [HttpPost]
        [Authorize]
        [Route("update-rating")]
        public async Task<IActionResult> UpdateRating([FromBody] AddRatingTitle input)
        {
            var title = await titleService.UpdateRating(input.Id, input.Rating, input.UserId);
            return Ok(title);
        }

        [HttpPost]
        [Authorize]
        [Route("remove-rating")]
        public async Task<IActionResult> RemoveRating([FromBody] RemoveRatingTitle input)
        {
            var title = await titleService.RemoveRating(input.Id, input.UserId);
            return Ok(title);
        }
    }
}
