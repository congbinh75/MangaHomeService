﻿using MangaHomeService.Models.FormData;
using MangaHomeService.Services.Interfaces;
using MangaHomeService.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                    await _userService.Add(inputUser.Name, inputUser.Email, inputUser.Password, "Visitor"); //Register as visitor
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
                        new Claim("Email", user?.Email ?? ""),
                        new Claim(ClaimTypes.Role, user?.Role?.Name ?? "")
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
        public async Task<IActionResult> ChangePassword(UserChangePasswordData inputData)
        {
            if (inputData != null)
            {
                if(string.IsNullOrEmpty(inputData.oldPassword) || string.IsNullOrEmpty(inputData.newPassword) 
                    || string.IsNullOrEmpty(inputData.repeatNewPassword)) 
                {
                    return BadRequest("Missing required fields");
                }

                if(inputData.newPassword != inputData.repeatNewPassword)
                {
                    return BadRequest("Password and confirmation are not matched");
                }

                string currentUserId = Functions.GetCurrentUserId();
                var user = _userService.Get(currentUserId);
                if (user != null)
                {
                    await _userService.Update(id: currentUserId, password: inputData.newPassword);
                }
                else
                {
                    return BadRequest();
                }
                return Ok();
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
                string currentUser = Functions.GetCurrentUserId();
                await _userService.Update(id : currentUser, profilePicture : profilePicture);
                return Ok();
            }
            else 
            { 
                return BadRequest(); 
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                string currentUserId = Functions.GetCurrentUserId();
                var user = _userService.Get(currentUserId);
                if (user != null)
                {
                    await _userService.Update(id: currentUserId, name: name);
                }
                else
                {
                    return BadRequest();
                }
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateEmail(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                string currentUserId = Functions.GetCurrentUserId();
                var user = _userService.Get(currentUserId);
                if (user != null)
                {
                    await _userService.Update(id: currentUserId, email: email, emailConfirmed: false);
                }
                else
                {
                    return BadRequest();
                }
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var user = _userService.Get(id);
                if (user != null)
                {
                    await _userService.Update(id: id, emailConfirmed: true);
                }
                else
                {
                    return BadRequest();
                }
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SendConfirmationEmail(string id)
        {
            throw new NotImplementedException();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AddRole(UserAddRoleData data)
        {
            await _userService.AddRole(data.Name, data.Description);
            return Ok();
        }
    }
}
