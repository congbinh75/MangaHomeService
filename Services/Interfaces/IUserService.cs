﻿using MangaHomeService.Models;
using Microsoft.AspNetCore.Mvc;

namespace MangaHomeService.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> Add(string name, string email, string password, string? roleName = null);
        Task<User?> Get(string id);
        Task<User?> Get(string email, string password);
        Task Update(string id, string? name = null, string? email = null, string? password = null, bool? emailConfirmed = null, 
            string? profilePicture = null, string? roleName = null);
        Task Delete(string id);
        Task<List<Permission>> GetPermissionsOfUser(string userId);
    }
}
