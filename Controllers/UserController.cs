using MangaHomeService.Models.FormDatas.User;
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
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<UserController> _stringLocalizer;
        private readonly IUserService _userService;
        private readonly ITokenInfoProvider _tokenInfoProvider;

        public UserController(
            IConfiguration configuration,
            IStringLocalizer<UserController> stringLocalizer,
            IUserService userService,
            ITokenInfoProvider tokenInfoProvider)
        {
            _configuration = configuration;
            _stringLocalizer = stringLocalizer;
            _userService = userService;
            _tokenInfoProvider = tokenInfoProvider;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(Register input)
        {
            try
            {
                input.Validate();
                await _userService.Add(input.Name, input.Email, input.Password, 2);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Login input)
        {
            try
            {
                if (!string.IsNullOrEmpty(input.Username) && !string.IsNullOrEmpty(input.Password))
                {
                    var user = await _userService.Get(input.Username, input.Password);

                    if (user != null)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"] ?? ""),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                            new Claim(ClaimTypes.NameIdentifier, user.Id),
                            new Claim(ClaimTypes.Name, user.Name ?? ""),
                            new Claim(ClaimTypes.Role, ((Enums.Role)user.Role).ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
                        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _configuration["Jwt:Issuer"],
                            _configuration["Jwt:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddDays(7),
                            signingCredentials: signIn);

                        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                    }
                    else
                    {
                        return BadRequest(_stringLocalizer["ERR_INVALID_CREDENTIALS"]);
                    }
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePassword input)
        {
            try
            {
                input.Validate();
                var currentUserName = _tokenInfoProvider.Name;
                var user = await _userService.Get(currentUserName, input.OldPassword);
                if (user != null)
                {
                    await _userService.Update(id: _tokenInfoProvider.Id, password: input.NewPassword);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (ArgumentException)
            {
                return BadRequest(_configuration["ERR_INVALID_INPUT_DATA"]);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(Update input)
        {
            try
            {

                if (input.ProfilePicture != null)
                {
                    if (4 * (input.ProfilePicture.Length / 3) > Constants.ProfilePictureBytesLimit)
                    {
                        // TO BE FIXED
                        return BadRequest("File size exceeded 2MB limit");
                    }
                }

                string currentUserId = _tokenInfoProvider.Id;
                await _userService.Update(id: currentUserId, email: input.Email, profilePicture: input.ProfilePicture);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SendConfirmationEmail()
        {
            try
            {
                await _userService.SendEmailConfirmation();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
            {
                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(token))
                {
                    await _userService.ConfirmEmail(userId, token);
                    return Ok();
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
    }
}
