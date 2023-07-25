using MangaHomeService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

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
    }
}
