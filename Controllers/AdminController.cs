using Microsoft.AspNetCore.Mvc;

namespace MangaHomeService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : Controller
    {
        public async Task<IActionResult> AddLanguage()
        {
            return View();
        }
    }
}
