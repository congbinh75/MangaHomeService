using MangaHomeService.Models;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace MangaHomeService.Services
{
    public interface IPersonService
    {
        public Task<Person> Get(string id);
        public Task<Person> Add(string name, IFormFile? image = null, string? description = null, List<string>? authoredTitlesIds = null,
            List<string>? illustratedTitlesIds = null);
        public Task<Person> Update(string id, string? name = null, IFormFile? image = null, string? description = null, 
            List<string>? authoredTitlesIds = null, List<string>? illustratedTitlesIds = null);
        public Task<bool> Delete(string id);
    }

    public class PersonService : IPersonService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;

        public PersonService(IDbContextFactory<MangaHomeDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Person> Get(string id)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var person = await dbContext.People.FirstOrDefaultAsync(x => x.Id == id);
                if (person == null)
                {
                    throw new NotFoundException(typeof(Person).Name);
                }
                return person;
            }
        }

        public async Task<Person> Add(string name, IFormFile? image = null, string? description = null, List<string>? authoredTitlesIds = null,
            List<string>? illustratedTitlesIds = null)
        {
            using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var authoredTitles = new List<Title>();
                if (authoredTitlesIds != null)
                {
                    foreach (string authoredTitleId in authoredTitlesIds)
                    {
                        var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == authoredTitleId);
                        if (title == null)
                        {
                            throw new NotFoundException(typeof(Title).Name);
                        }
                        authoredTitles.Add(title);
                    }
                }

                var illustratedTitles = new List<Title>();
                if (illustratedTitlesIds != null)
                {
                    foreach (string illustratedTitleId in illustratedTitlesIds)
                    {
                        var title = await dbContext.Titles.FirstOrDefaultAsync(t => t.Id == illustratedTitleId);
                        if (title == null)
                        {
                            throw new NotFoundException(typeof(Title).Name);
                        }
                        illustratedTitles.Add(title);
                    }
                }

                var person = new Person();
                person.Name = name;
                person.Description = description == null ? "" : description;
                person.AuthoredTitles = authoredTitles;
                person.IllustratedTitles = illustratedTitles;
                await dbContext.People.AddAsync(person);
                await dbContext.SaveChangesAsync();
                return person;
            }
        }

        public Task<Person> Update(string id, string? name = null, IFormFile? image = null, string? description = null,
            List<string>? authoredTitlesIds = null, List<string>? illustratedTitlesIds = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}
