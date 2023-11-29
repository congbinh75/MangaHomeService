using MangaHomeService.Models;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Services
{
    public interface IPersonService
    {
        public Task<Person> Get(string id);
        public Task<Person> Add(string name, IFormFile? image = null, string? description = null,
            ICollection<string>? authoredTitlesIds = null, ICollection<string>? illustratedTitlesIds = null);
        public Task<Person> Update(string id, string? name = null, IFormFile? image = null, string? description = null,
            ICollection<string>? authoredTitlesIds = null, ICollection<string>? illustratedTitlesIds = null);
        public Task<bool> Remove(string id);
    }

    public class PersonService : IPersonService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;
        private readonly IConfiguration _configuration;

        public PersonService(IDbContextFactory<MangaHomeDbContext> contextFactory, IConfiguration configuration)
        {
            _contextFactory = contextFactory;
            _configuration = configuration;
        }

        public async Task<Person> Get(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var person = await dbContext.People.FirstOrDefaultAsync(x => x.Id == id) ?? throw new NotFoundException(nameof(Person));
            return person;
        }

        public async Task<Person> Add(string name, IFormFile? image = null, string? description = null,
            ICollection<string>? authoredTitlesIds = null, ICollection<string>? illustratedTitlesIds = null)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var authoredTitles = new List<Title>();
            if (authoredTitlesIds != null)
            {
                foreach (string authoredTitleId in authoredTitlesIds)
                {
                    var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == authoredTitleId) ??
                        throw new NotFoundException(nameof(Title));
                    authoredTitles.Add(title);
                }
            }

            var illustratedTitles = new List<Title>();
            if (illustratedTitlesIds != null)
            {
                foreach (string illustratedTitleId in illustratedTitlesIds)
                {
                    var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == illustratedTitleId) ??
                        throw new NotFoundException(nameof(Title));
                    illustratedTitles.Add(title);
                }
            }

            var person = new Person
            {
                Name = name,
                Image = image == null ? "" : await Functions.UploadFileAsync(image, _configuration["FilesStoragePath.PeopleImagesPath"]),
                Description = description ?? "",
                AuthoredTitles = authoredTitles,
                IllustratedTitles = illustratedTitles
            };
            await dbContext.People.AddAsync(person);
            await dbContext.SaveChangesAsync();
            return person;
        }

        public async Task<Person> Update(string id, string? name = null, IFormFile? image = null, string? description = null,
            ICollection<string>? authoredTitlesIds = null, ICollection<string>? illustratedTitlesIds = null)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var person = await dbContext.People.FirstOrDefaultAsync(p => p.Id == id) ?? throw new NotFoundException(nameof(Person));

            var authoredTitles = new List<Title>();
            if (authoredTitlesIds != null)
            {
                foreach (string authoredTitleId in authoredTitlesIds)
                {
                    var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == authoredTitleId) ??
                        throw new NotFoundException(nameof(Title));
                    authoredTitles.Add(title);
                }
            }

            var illustratedTitles = new List<Title>();
            if (illustratedTitlesIds != null)
            {
                foreach (string illustratedTitleId in illustratedTitlesIds)
                {
                    var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == illustratedTitleId) ??
                        throw new NotFoundException(nameof(Title));
                    illustratedTitles.Add(title);
                }
            }

            person.Name = name ?? person.Name;
            person.Image = image == null ? person.Image :
                await Functions.UploadFileAsync(image, _configuration["FilesStoragePath.PeopleImagesPath"]);
            person.Description = description ?? person.Description;
            person.AuthoredTitles = authoredTitles;
            person.IllustratedTitles = illustratedTitles;
            await dbContext.SaveChangesAsync();
            return person;
        }

        public async Task<bool> Remove(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var person = await dbContext.People.FirstOrDefaultAsync(p => p.Id == id) ??
                throw new NotFoundException(nameof(Person));
            dbContext.Remove(person);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
