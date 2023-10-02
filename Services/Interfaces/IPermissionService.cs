﻿using MangaHomeService.Models;

namespace MangaHomeService.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<Permission?> Get(string id);
        Task<List<Permission>> GetAll();
        Task<Permission> Add(string name, string description);
        Task<Permission> Update(string id, string name, string description);
        Task<bool> Remove(string id);
    }
}