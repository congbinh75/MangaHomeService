using MangaHomeService.Models;
using MangaHomeService.Utils;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace MangaHomeService.Services
{
    public interface IUserService
    {
        public Task<User> Add(string name, string email, string password, int role);
        public Task<User> Get(string id);
        public Task<User?> Get(string email, string password);
        public Task<User> Update(string id, string? name = null, string? email = null, string? password = null, int? role = null,
            bool? isEmailConfirmed = null, IFormFile? profilePicture = null, bool? isBanned = null);
        public Task<bool> Delete(string id);
        public Task<bool> SendEmailConfirmation(string? userId = null);
        public Task<bool> ConfirmEmail(string userId, string token);
    }

    public class UserService : IUserService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;
        private readonly IConfiguration _configuration;
        private readonly ITokenInfoProvider _tokenInfoProvider;
        private readonly IHttpClientFactory _httpClientFactory;

        public UserService(IDbContextFactory<MangaHomeDbContext> contextFactory, IConfiguration configuration, ITokenInfoProvider tokenInfoProvider, IHttpClientFactory httpClientFactory)
        {
            _contextFactory = contextFactory;
            _configuration = configuration;
            _tokenInfoProvider = tokenInfoProvider;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<User> Get(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var user = await dbContext.Users.Where(u => u.Id == id).FirstOrDefaultAsync() ?? throw new NotFoundException(nameof(User));
            return user;
        }

        public async Task<User?> Get(string email, string password)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var user = await dbContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (user != null && HashPassword(password, user.Salt).hashedPassword.Equals(user.Password))
            {
                return user;
            }
            else
            {
                return null;
            }
        }

        public async Task<User> Add(string name, string email, string password, int role)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                throw new EmailAlreadyRegisteredException();
            }

            (string hashed, byte[] salt) passAndSalt = HashPassword(password);
            var user = new User
            {
                Name = name,
                Email = email,
                Password = passAndSalt.hashed,
                IsEmailConfirmed = false,
                ProfilePicture = "",
                Salt = passAndSalt.salt,
                Role = role,
                IsBanned = false
            };
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> Update(string userId, string? name = null, string? email = null, string? password = null, int? role = null,
            bool? isEmailConfirmed = null, IFormFile? profilePicture = null, bool? isBanned = null)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId) ??
                throw new NotFoundException(nameof(User));
            var newPassword = user.Password;
            var newSalt = user.Salt;
            if (password != null)
            {
                (string hassed, byte[] salt) passAndSalt = HashPassword(password);
                newPassword = passAndSalt.hassed;
                newSalt = passAndSalt.salt;
            }

            user.Name = name == null ? user.Name : name;
            user.Email = email == null ? user.Email : email;
            user.Role = role == null ? user.Role : (int)role;
            user.IsEmailConfirmed = isEmailConfirmed == null ? user.IsEmailConfirmed : (bool)isEmailConfirmed;
            user.ProfilePicture = profilePicture == null ? user.ProfilePicture :
                await Functions.UploadFileAsync(profilePicture, _configuration["FilesStoragePath.ProfilePicturesPath"]);
            user.Password = newPassword;
            user.Salt = newSalt;
            user.IsBanned = isBanned == null ? user.IsBanned : (bool)isBanned;

            await dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<bool> Delete(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id) ?? throw new NotFoundException(nameof(User));
            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SendEmailConfirmation(string? userId = null)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var id = userId ?? _tokenInfoProvider.Id;
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id) ?? throw new NotFoundException(nameof(User));
            if (user.IsEmailConfirmed)
            {
                throw new EmailAlreadyConfirmedException();
            }
            string token = Guid.NewGuid().ToString();
            user.EmailConfirmationToken = token;
            await dbContext.SaveChangesAsync();

            using var httpClient =  _httpClientFactory.CreateClient();
            using var request = new HttpRequestMessage(new HttpMethod("POST"), _configuration["SMTP.Url"]);
            request.Headers.TryAddWithoutValidation("api-key", _configuration["SMTP.APIKey"]);

            //TO BE FIXED
            request.Content = new StringContent("");
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

            var response = await httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ConfirmEmail(string userId, string token)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new NotFoundException(nameof(User));
            if (user.IsEmailConfirmed)
            {
                throw new EmailAlreadyConfirmedException();
            }

            if (user.EmailConfirmationToken == token) 
            {
                user.IsEmailConfirmed = true;
                await dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new Exception();
            }
        }

        private static (string hashedPassword, byte[] salt) HashPassword(string password, byte[]? salt = null)
        {
            salt ??= RandomNumberGenerator.GetBytes(128 / 8);
            string hassed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));
            return (hassed, salt);
        }
    }
}
