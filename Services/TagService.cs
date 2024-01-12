using MangaHomeService.Models;
using MangaHomeService.Models.Entities;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Services
{
    public interface ITagService
    {
        public Task<Tag> Get(string id);
        public Task<ICollection<Tag>> GetByType(Type type);
        public Task<Tag> Add(string name, Type type, string? description = null, ICollection<string>? titlesIds = null,
            ICollection<string>? otherNamesIds = null);
        public Task<Tag> Update(string id, string? name = null, string? description = null, ICollection<string>? titlesIds = null, 
            ICollection<string>? otherNamesIds = null);
        public Task<bool> Remove(string id);
    }

    public class TagService(IDbContextFactory<MangaHomeDbContext> contextFactory) : ITagService
    {
        public async Task<Tag> Get(string id)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var tag = await dbContext.Tags.FirstOrDefaultAsync(x => x.Id == id) ?? throw new NotFoundException(nameof(Tag));
            return tag;
        }

        public async Task<ICollection<Tag>> GetByType(Type type)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            if (type == typeof(Gerne))
            {
                return (ICollection<Tag>)await dbContext.Tags.OfType<Gerne>().ToListAsync();
            }
            else if (type == typeof(Theme))
            {
                return (ICollection<Tag>)await dbContext.Tags.OfType<Theme>().ToListAsync();
            }
            else if (type == typeof(Demographic))
            {
                return (ICollection<Tag>)await dbContext.Tags.OfType<Demographic>().ToListAsync();
            }
            else
            {
                throw new ArgumentException(type.ToString());
            }
        }

        public async Task<Tag> Add(string name, Type type, string? description = null,
            ICollection<string>? titlesIds = null, ICollection<string>? otherNamesIds = null)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
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

            var otherNames = new List<OtherName>();
            if (otherNamesIds != null)
            {
                foreach (string otherNameId in otherNamesIds)
                {
                    var otherName = await dbContext.OtherNames.FirstOrDefaultAsync(t => t.Id == otherNameId) ??
                        throw new NotFoundException(nameof(OtherName));
                    otherNames.Add(otherName);
                }
            }

            //TO BE FIXED
            var tag = new Gerne
            {
                Name = name,
                Description = description ?? "",
                Titles = titles,
                OtherNames = otherNames
            };
            await dbContext.AddAsync(tag);
            await dbContext.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag> Update(string id, string? name = null, string? description = null, ICollection<string>? titlesIds = null, 
            ICollection<string>? otherNamesIds = null)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var tag = await dbContext.Tags.FirstOrDefaultAsync(t => t.Id == id) ?? throw new NotFoundException(nameof(Tag));
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

            var otherNames = new List<OtherName>();
            if (otherNamesIds != null)
            {
                foreach (string otherNameId in otherNamesIds)
                {
                    var otherName = await dbContext.OtherNames.FirstOrDefaultAsync(t => t.Id == otherNameId) ??
                        throw new NotFoundException(nameof(OtherName));
                    otherNames.Add(otherName);
                }
            }

            tag.Name = name ?? tag.Name;
            tag.Description = description ?? "";
            //TO BE FIXED
            tag.OtherNames = otherNamesIds == null ? tag.OtherNames : otherNames;
            await dbContext.SaveChangesAsync();
            return tag;
        }

        public async Task<bool> Remove(string id)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var tag = await dbContext.Tags.FirstOrDefaultAsync(x => x.Id == id) ?? throw new NotFoundException(nameof(Tag));
            dbContext.Tags.Remove(tag);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
