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
        private IConfiguration _configuration;
        private IStringLocalizer<UserController> _stringLocalizer;
        private IUserService _userService;
        private ITokenInfoProvider _tokenInfoProvider;

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
                if (input != null)
                {
                    if (string.IsNullOrEmpty(input.OldPassword) || string.IsNullOrEmpty(input.NewPassword)
                        || string.IsNullOrEmpty(input.RepeatNewPassword))
                    {
                        return BadRequest(_stringLocalizer["ERR_INVALID_INPUT_DATA"]);
                    }

                    if (input.NewPassword != input.RepeatNewPassword)
                    {
                        return BadRequest(_stringLocalizer["ERR_INVALID_INPUT_DATA"]);
                    }

                    string currentUserId = _tokenInfoProvider.Id;
                    var user = _userService.Get(currentUserId);
                    if (user != null)
                    {
                        await _userService.Update(id: currentUserId, password: input.NewPassword);
                    }
                    else
                    {
                        return NotFound();
                    }
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateProfilePicture(IFormFile profilePicture)
        {
            try
            {
                if (profilePicture != null)
                {
                    if (4 * (profilePicture.Length / 3) > Constants.ProfilePictureBytesLimit)
                    {
                        // TO BE FIXED
                        return BadRequest("File size exceeded 2MB limit");
                    }
                    string currentUserId = _tokenInfoProvider.Id;
                    await _userService.Update(id: currentUserId, profilePicture: profilePicture);
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateInfo(string email)
        {
            try
            {
                if (!string.IsNullOrEmpty(email))
                {
                    string currentUserId = _tokenInfoProvider.Id;
                    var user = _userService.Get(currentUserId);
                    if (user != null)
                    {
                        await _userService.Update(id: currentUserId, email: email);
                    }
                    else
                    {
                        return BadRequest();
                    }
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendConfirmationEmail()
        {
            try
            {

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ConfirmEmail()
        {
            throw new NotImplementedException();
        }
    }
}
