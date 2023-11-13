﻿using MangaHomeService.Models;
using MangaHomeService.Services.Interfaces;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Services
{
    public class ReadingListService : IReadingListService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;

        public ReadingListService(IDbContextFactory<MangaHomeDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<ReadingList> Get(string id)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var list = await dbContext.ReadingLists.FirstOrDefaultAsync(r => r.Id == id);
                if (list == null)
                {
                    throw new NotFoundException(typeof(ReadingList).Name);
                }
                if (!list.IsPublic) 
                {
                    var currentUser = Functions.GetCurrentUserId();
                    if (list.User.Id != currentUser)
                    {
                        throw new Exception();
                    }
                }
                return list;
            }
        }
        public async Task<List<ReadingList>> GetAll(string? userId = null)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var currentUser = Functions.GetCurrentUserId();
                if (userId == null)
                {
                    return await dbContext.ReadingLists.Where(r => r.User.Id == currentUser).ToListAsync();
                }
                else
                {
                    var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                    if (user == null)
                    {
                        throw new NotFoundException(typeof(User).Name);
                    }
                    if (user.Id == currentUser)
                    {
                        return await dbContext.ReadingLists.Where(r => r.User.Id == currentUser).ToListAsync();
                    }
                    else
                    {
                        return await dbContext.ReadingLists.Where(r => r.User.Id == currentUser && r.IsPublic == true).ToListAsync();
                    }
                }
            }
        }

        public async Task<ReadingList> Add(string name, string? userId = null, string description = "", bool isPublic = false, 
            List<string>? titlesIds = null)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                string ownerId = userId == null ? Functions.GetCurrentUserId() : userId;
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == ownerId);
                if (user == null)
                {
                    throw new NotFoundException(typeof(User).Name);
                }

                var titles = new List<Title>();
                if (titlesIds != null)
                {
                    foreach (string titleId in titlesIds)
                    {
                        var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == titleId);
                        if (title == null)
                        {
                            throw new NotFoundException(typeof(Title).Name);
                        }
                        titles.Add(title);
                    }
                }

                var list = new ReadingList();
                list.Name = name;
                list.User = user;
                list.Description = description;
                list.IsPublic = isPublic;
                list.Titles = titles;
                await dbContext.ReadingLists.AddAsync(list);
                await dbContext.SaveChangesAsync();
                return list;
            }
        }

        public async Task<ReadingList> Update(string id, string? userId = null, string? name = null, string? description = null, 
            bool? isPublic = null, List<string>? titlesIds = null)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var list = await dbContext.ReadingLists.FirstOrDefaultAsync(r => r.Id == id);
                if (list == null)
                {
                    throw new NotFoundException(typeof(ReadingList).Name);
                }

                var newUser = list.User;
                if (userId != null)
                {
                    newUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                    if (newUser == null)
                    {
                        throw new NotFoundException(typeof(User).Name);
                    }
                }

                var newTitles = new List<Title>();
                if (titlesIds != null)
                {
                    foreach (string titleId in titlesIds)
                    {
                        var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == titleId);
                        if (title == null)
                        {
                            throw new NotFoundException(typeof(Title).Name);
                        }
                        newTitles.Add(title);
                    }
                }

                list.Name = name == null ? list.Name : name;
                list.User = userId == null ? list.User : newUser;
                list.Description = description == null ? list.Description : description;
                list.IsPublic = isPublic == null ? list.IsPublic : (bool)isPublic;
                list.Titles = titlesIds == null ? list.Titles : newTitles;
                await dbContext.ReadingLists.AddAsync(list);
                await dbContext.SaveChangesAsync();
                return list;
            }
        }

        public async Task<bool> Delete(string id)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var list = await dbContext.ReadingLists.FirstOrDefaultAsync(r => r.Id == id);
                if (list == null)
                {
                    throw new NotFoundException(typeof(ReadingList).Name);
                }
                dbContext.ReadingLists.Remove(list);
                await dbContext.SaveChangesAsync();
                return true;
            }
        }
    }
}
