﻿using Microsoft.AspNetCore.Mvc;

namespace MangaHomeService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> AddLanguage()
        {
            throw new NotImplementedException();
        }
    }
}
