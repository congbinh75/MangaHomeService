using MangaHomeService.Models.InputModels;
using MangaHomeService.Services;
using MangaHomeService.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MangaHomeService.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController(IConfiguration configuration, IStringLocalizer<SharedResources> stringLocalizer, IUserService userService, 
        ITokenInfoProvider tokenInfoProvider) : ControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser input)
        {
            await userService.Add(input.Name, input.Email, input.Password, 2);
            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser input)
        {
            var user = await userService.Get(input.Username, input.Password);

            if (user != null)
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"] ?? ""),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.Username ?? ""),
                    new Claim(ClaimTypes.Role, ((Enums.Role)user.Role).ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? ""));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    configuration["Jwt:Issuer"],
                    configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddDays(7),
                    signingCredentials: signIn);

                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
            else
            {
                return BadRequest(stringLocalizer[Constants.ERR_INVALID_CREDENTIALS].Value);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordUser input)
        {
            var user = await userService.Get(tokenInfoProvider.Name, input.OldPassword);
            if (user != null)
            {
                await userService.Update(id: tokenInfoProvider.Id, password: input.NewPassword);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize]
        [Route("update")]
        public async Task<IActionResult> Update(UpdateUser input)
        {
            if (input.ProfilePicture != null)
            {
                if (4 * (input.ProfilePicture.Length / 3) > Constants.ProfilePictureBytesLimit)
                {
                    // TO BE FIXED
                    return BadRequest("File size exceeded 2MB limit");
                }
            }

            await userService.Update(id: tokenInfoProvider.Id, email: input.Email, profilePicture: input.ProfilePicture);
            return Ok();
        }

        [HttpGet]
        [Authorize]
        [Route("send-confirmation-email")]
        public async Task<IActionResult> SendConfirmationEmail()
        {
            await userService.SendEmailConfirmation();
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, string token)
        {
            await userService.ConfirmEmail(userId, token);
            return Ok();
        }
    }
}
