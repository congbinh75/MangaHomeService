using MangaHomeService.Models;
using MangaHomeService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MangaHomeService.Services
{
    public class TitleService : ITitleService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;

        public TitleService(IDbContextFactory<MangaHomeDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Title> Get(string id)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentNullException(nameof(id));
                }
                var title = await dbContext.Titles.Where(x => x.Id == id).Include(x => x.Chapters).FirstOrDefaultAsync();
                if (title == null) 
                {
                    throw new Exception();
                }
                return title;
            }
        }

        public Task<List<Title>> Search(string keyword, int count, int page)
        {
            throw new NotImplementedException();
        }
    }
}
