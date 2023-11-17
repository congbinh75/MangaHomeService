﻿using MangaHomeService.Models;
using MangaHomeService.Utils;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Services
{
    public interface ITagService
    {
        public Task<Tag> Get(string id);
        public Task<ICollection<Tag>> GetByType(int type);
        public Task<Tag> Add(string name, int type, string? description = null, List<string>? titlesIds = null,
            List<string>? otherNamesIds = null);
        public Task<Tag> Update(string id, string? name = null, int? type = null, string? description = null, List<string>? titlesIds = null,
            List<string>? otherNamesIds = null);
        public Task<bool> Delete(string id);
    }

    public class TagService : ITagService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;

        public TagService(IDbContextFactory<MangaHomeDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Tag> Get(string id)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var tag = await dbContext.Tags.FirstOrDefaultAsync(x => x.Id == id);
                if (tag == null)
                {
                    throw new NotFoundException(typeof(Tag).Name);
                }
                return tag;
            }
        }

        public async Task<ICollection<Tag>> GetByType(int type)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                return await dbContext.Tags.Where(x => x.Type == type).ToListAsync();
            }
        }

        public async Task<Tag> Add(string name, int type, string? description = null, List<string>? titlesIds = null,
            List<string>? otherNamesIds = null)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
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

                var otherNames = new List<OtherName>();
                if (otherNamesIds != null)
                {
                    foreach (string otherNameId in otherNamesIds)
                    {
                        var otherName = await dbContext.OtherNames.FirstOrDefaultAsync(t => t.Id == otherNameId);
                        if (otherName == null)
                        {
                            throw new NotFoundException(typeof(OtherName).Name);
                        }
                        otherNames.Add(otherName);
                    }
                }


                var tag = new Tag();
                tag.Name = name;
                tag.Description = description == null ? "" : description;
                tag.Type = type;
                tag.Titles = titles;
                tag.OtherNames = otherNames;
                await dbContext.AddAsync(tag);
                await dbContext.SaveChangesAsync();
                return tag;
            }
        }

        public async Task<Tag> Update(string id, string? name = null, int? type = null, string? description = null, 
            List<string>? titlesIds = null, List<string>? otherNamesIds = null)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var tag = await dbContext.Tags.FirstOrDefaultAsync(t => t.Id == id);
                if (tag == null)
                {
                    throw new NotFoundException(typeof(Tag).Name);
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

                var otherNames = new List<OtherName>();
                if (otherNamesIds != null)
                {
                    foreach (string otherNameId in otherNamesIds)
                    {
                        var otherName = await dbContext.OtherNames.FirstOrDefaultAsync(t => t.Id == otherNameId);
                        if (otherName == null)
                        {
                            throw new NotFoundException(typeof(OtherName).Name);
                        }
                        otherNames.Add(otherName);
                    }
                }

                tag.Name = name == null ? tag.Name : name;
                tag.Description = description == null ? "" : description;
                tag.Type = type == null ? tag.Type : (int)type;
                tag.Titles = titlesIds == null ? tag.Titles : titles;
                tag.OtherNames = otherNamesIds == null ? tag.OtherNames : otherNames;
                await dbContext.SaveChangesAsync();
                return tag;
            }
        }

        public async Task<bool> Delete(string id)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var tag = await dbContext.Tags.FirstOrDefaultAsync(x => x.Id == id);
                if (tag == null)
                {
                    throw new NotFoundException(typeof(Tag).Name);
                }
                dbContext.Tags.Remove(tag);
                await dbContext.SaveChangesAsync();
                return true;
            }
        }
    }
}
