using MangaHomeService.Models.FormData;
using MangaHomeService.Services.Interfaces;
using MangaHomeService.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static MangaHomeService.Utils.Enums;

namespace MangaHomeService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IConfiguration _configuration;
        private IUserService _userService;

        public UserController(IConfiguration configuration, IUserService userService) 
        {
            _configuration = configuration;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterData inputUser)
        {
            try
            {
                if (!string.IsNullOrEmpty(inputUser.Email) && !string.IsNullOrEmpty(inputUser.Password) && !string.IsNullOrEmpty(inputUser.Name))
                {
                    await _userService.Add(inputUser.Name, inputUser.Email, inputUser.Password, 4); //Register as visitor
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginData inputUser)
        {
            if(!string.IsNullOrEmpty(inputUser.Email) && !string.IsNullOrEmpty(inputUser.Password)) 
            {
                var user = await _userService.Get(inputUser.Email, inputUser.Password);

                if (user != null) 
                {
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", user?.Id ?? ""),
                        new Claim("DisplayName", user?.Name ?? ""),
                        new Claim("Email", user ?.Email ?? ""),
                        new Claim(ClaimTypes.Role, ((Roles)(user?.Role ?? 4)).ToString())
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddSeconds(10),
                        signingCredentials: signIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else 
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfilePicture(string profilePicture)
        {
            if (!string.IsNullOrEmpty(profilePicture)) 
            {
                if (4 * (profilePicture.Length / 3) > Constants.ProfilePictureBytesLimit)
                {
                    return BadRequest("File size exceeded 2MB limit");
                }
                string currentUser = "";
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    currentUser = claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "";
                }
                else
                {
                    throw new Exception();
                }
                await _userService.UpdateProfilePicture(currentUser, profilePicture);
                return Ok();
            }
            else 
            { 
                return BadRequest(); 
            }
        }

    }
}
