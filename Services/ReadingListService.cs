using MangaHomeService.Models;
using MangaHomeService.Models.Entities;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Services
{
    public interface IReadingListService
    {
        public Task<ReadingList> Get(string id);
        public Task<ICollection<ReadingList>> GetAll(string? userId = null);
        public Task<ReadingList> Add(string name, string? userId = null, string description = "", bool isPublic = false,
            ICollection<string>? titlesIds = null);
        public Task<ReadingList> Update(string id, string? userId = null, string? name = null, string? description = null,
            bool? isPublic = null, ICollection<string>? titlesIds = null);
        public Task<bool> Remove(string id);
        public Task<ReadingList> AddTitle(string id, string titleId);
        public Task<ReadingList> RemoveTitle(string id, string titleId);
    }

    public class ReadingListService(IDbContextFactory<MangaHomeDbContext> contextFactory, ITokenInfoProvider tokenInfoProvider) : IReadingListService
    {
        public async Task<ReadingList> Get(string id)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var list = await dbContext.ReadingLists.FirstOrDefaultAsync(r => r.Id == id) ??
                throw new NotFoundException(nameof(ReadingList));
            if (!list.IsPublic)
            {
                var currentUser = tokenInfoProvider.Id;
                if (list?.Owner?.Id != currentUser)
                {
                    throw new Exception();
                }
            }
            return list;
        }
        public async Task<ICollection<ReadingList>> GetAll(string? userId = null)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var currentUser = tokenInfoProvider.Id;
            if (userId == null)
            {
                return await dbContext.ReadingLists.Where(r => r.Owner.Id == currentUser).ToListAsync();
            }
            else
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId) ??
                    throw new NotFoundException(nameof(User));
                if (user.Id == currentUser)
                {
                    return await dbContext.ReadingLists.Where(r => r.Owner.Id == currentUser).ToListAsync();
                }
                else
                {
                    return await dbContext.ReadingLists.Where(r => r.Owner.Id == currentUser && r.IsPublic == true).ToListAsync();
                }
            }
        }

        public async Task<ReadingList> Add(string name, string? userId = null, string description = "", bool isPublic = false,
            ICollection<string>? titlesIds = null)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            string ownerId = userId ?? tokenInfoProvider.Id;
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == ownerId) ?? throw new NotFoundException(nameof(User));
            var titles = new List<Title>();
            if (titlesIds != null)
            {
                foreach (string titleId in titlesIds)
                {
                    var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == titleId) ??
                        throw new NotFoundException(nameof(Title));
                    titles.Add(title);
                }
            }

            var list = new ReadingList
            {
                Name = name,
                Owner = user,
                Description = description,
                IsPublic = isPublic,
                Titles = titles
            };
            await dbContext.ReadingLists.AddAsync(list);
            await dbContext.SaveChangesAsync();
            return list;
        }

        public async Task<ReadingList> Update(string id, string? userId = null, string? name = null, string? description = null,
            bool? isPublic = null, ICollection<string>? titlesIds = null)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var list = await dbContext.ReadingLists.FirstOrDefaultAsync(r => r.Id == id) ??
                throw new NotFoundException(nameof(ReadingList));
            var newUser = list.Owner;
            if (userId != null)
            {
                newUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new NotFoundException(nameof(User));
            }

            var newTitles = new List<Title>();
            if (titlesIds != null)
            {
                foreach (string titleId in titlesIds)
                {
                    var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == titleId) ??
                        throw new NotFoundException(nameof(Title));
                    newTitles.Add(title);
                }
            }

            list.Name = name ?? list.Name;
            list.Owner = userId == null ? list.Owner : newUser;
            list.Description = description ?? list.Description;
            list.IsPublic = isPublic == null ? list.IsPublic : (bool)isPublic;
            list.Titles = titlesIds == null ? list.Titles : newTitles;
            await dbContext.ReadingLists.AddAsync(list);
            await dbContext.SaveChangesAsync();
            return list;
        }

        public async Task<bool> Remove(string id)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var list = await dbContext.ReadingLists.FirstOrDefaultAsync(r => r.Id == id) ??
                throw new NotFoundException(nameof(ReadingList));
            dbContext.ReadingLists.Remove(list);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<ReadingList> AddTitle(string id, string titleId)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var list = await dbContext.ReadingLists.Where(r => r.Id == id && r.Titles != null).Include(r => r.Titles).FirstOrDefaultAsync() ??
                throw new NotFoundException(nameof(ReadingList));
            if (list.Titles.FirstOrDefault(t => t.Id == titleId) != null)
            {
                //TO BE FIXED
                throw new Exception();
            }

            var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == titleId) ??
                throw new NotFoundException(nameof(Title));
            list.Titles.Add(title);
            await dbContext.SaveChangesAsync();
            return list;
        }

        public async Task<ReadingList> RemoveTitle(string id, string titleId)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var list = await dbContext.ReadingLists.Where(r => r.Id == id && r.Titles != null).Include(r => r.Titles).FirstOrDefaultAsync() ??
                throw new NotFoundException(nameof(ReadingList));
            //TO BE FIXED
            var titleInList = list.Titles.FirstOrDefault(t => t.Id == titleId) ?? throw new Exception();
            list.Titles.Add(titleInList);
            await dbContext.SaveChangesAsync();
            return list;
        }
    }
}
