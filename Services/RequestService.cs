using MangaHomeService.Models;
using MangaHomeService.Models.Entities;
using MangaHomeService.Models.InputModels;
using MangaHomeService.Utils;
using Microsoft.EntityFrameworkCore;
using Group = MangaHomeService.Models.Entities.Group;

namespace MangaHomeService.Services
{
    public interface IRequestService
    {
        public Task<Request> Get(string id);
        public Task<ICollection<Request>> GetAll(int? pageNumber = 1, int? pageSize = Constants.RequestsPerPage, int? requestType = null);
        public Task<GroupRequest> Submit(GroupRequestData data);
        public Task<ChapterRequest> Submit(ChapterRequestData data);
        public Task<TitleRequest> Submit(TitleRequestData data);
        public Task<MemberRequest> Submit(MemberRequestData data);
        public Task<AuthorRequest> Submit(AuthorRequestData data);
        public Task<ArtistRequest> Submit(ArtistRequestData data);
        public Task<Request> Update(string id, string note);
        public Task<Request> Review(string id, string note, bool isApproved);
        public Task<bool> Remove(string id);
    }

    public class RequestService : IRequestService
    {
        private readonly IDbContextFactory<MangaHomeDbContext> _contextFactory;
        private readonly ITokenInfoProvider _tokenInfoProvider;

        public RequestService(IDbContextFactory<MangaHomeDbContext> contextFactory, ITokenInfoProvider tokenInfoProvider)
        {
            _contextFactory = contextFactory;
            _tokenInfoProvider = tokenInfoProvider;
        }

        public async Task<Request> Get(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var request = await dbContext.Requests.Where(r => r.Id == id)
                .Include(r => (r as ChapterRequest).Chapter)
                .Include(r => (r as ChapterRequest).Group)
                .Include(r => (r as TitleRequest).Title)
                .Include(r => (r as TitleRequest).Group)
                .Include(r => (r as MemberRequest).Member)
                .Include(r => (r as MemberRequest).Group)
                .Include(r => (r as GroupRequest).Group)
                .Include(r => (r as ArtistRequest).Artist)
                .Include(r => (r as AuthorRequest).Author)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException();
            return request;
        }

        public async Task<GroupRequest> GetGroupRequest(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var request = await dbContext.Requests.Where(r => r.Id == id)
                .OfType<GroupRequest>()
                .Include(r => r.Group)
                .FirstOrDefaultAsync()
                ?? throw new Exception(nameof(GroupRequest));
            return request;
        }

        public async Task<MemberRequest> GetMemberRequest(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var request = await dbContext.Requests.Where(r => r.Id == id)
                .OfType<MemberRequest>()
                .Include(r => r.Member)
                .Include(r => r.Group)
                .FirstOrDefaultAsync()
                ?? throw new Exception(nameof(MemberRequest));
            return request;
        }

        public async Task<TitleRequest> GetTitleRequest(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var request = await dbContext.Requests.Where(r => r.Id == id)
                .OfType<TitleRequest>()
                .Include(r => r.Title)
                .Include(r => r.Group)
                .FirstOrDefaultAsync()
                ?? throw new Exception(nameof(TitleRequest));
            return request;
        }

        public async Task<ChapterRequest> GetChapterRequest(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var request = await dbContext.Requests.Where(r => r.Id == id)
                .OfType<ChapterRequest>()
                .Include(r => r.Chapter)
                .Include(r => r.Group)
                .FirstOrDefaultAsync()
                ?? throw new Exception(nameof(ChapterRequest));
            return request;
        }

        public async Task<AuthorRequest> GetAuthorRequest(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var request = await dbContext.Requests.Where(r => r.Id == id)
                .OfType<AuthorRequest>()
                .Include(r => r.Author)
                .FirstOrDefaultAsync()
                ?? throw new Exception(nameof(AuthorRequest));
            return request;
        }

        public async Task<ArtistRequest> GetArtistRequest(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var request = await dbContext.Requests.Where(r => r.Id == id)
                .OfType<ArtistRequest>()
                .Include(r => r.Artist)
                .FirstOrDefaultAsync()
                ?? throw new Exception(nameof(ArtistRequest));
            return request;
        }

        public async Task<GroupRequest> Submit(GroupRequestData data)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == data.GroupId) ??
                throw new NotFoundException(nameof(Group));
            if (group.IsApproved)
            {
                throw new AlreadyApprovedException(nameof(Group));
            }
            var request = new GroupRequest
            {
                Group = group,
                SubmitNote = data.SubmitNote,
                IsApproved = false,
                IsReviewed = false
            };
            await dbContext.Requests.AddAsync(request);
            await dbContext.SaveChangesAsync();
            return request;
        }

        public async Task<MemberRequest> Submit(MemberRequestData data)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var member = await dbContext.Members.Where(t => t.Id == data.MemberId && t.IsApproved == false).FirstOrDefaultAsync() ??
                throw new NotFoundException(nameof(Member));
            var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == data.GroupId) ??
                throw new NotFoundException(nameof(Group));
            var request = new MemberRequest
            {
                Member = member,
                Group = group,
                SubmitNote = data.SubmitNote
            };

            await dbContext.Requests.AddAsync(request);
            await dbContext.SaveChangesAsync();
            return request;
        }

        public async Task<TitleRequest> Submit(TitleRequestData data)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var title = await dbContext.Titles.Where(t => t.Id == data.TitleId && t.IsApproved == false).FirstOrDefaultAsync() ??
                throw new NotFoundException(nameof(Title));
            var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == data.GroupId) ??
                throw new NotFoundException(nameof(Group));
            var request = new TitleRequest
            {
                Title = title,
                Group = group,
                SubmitNote = data.SubmitNote
            };

            await dbContext.Requests.AddAsync(request);
            await dbContext.SaveChangesAsync();
            return request;
        }

        public async Task<ChapterRequest> Submit(ChapterRequestData data)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var chapter = await dbContext.Chapters.FirstOrDefaultAsync(t => t.Id == data.ChapterId) ??
                throw new NotFoundException(nameof(Chapter));
            if (chapter.IsApproved)
            {
                throw new AlreadyApprovedException(nameof(Chapter));
            }
            var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == data.GroupId) ??
                throw new NotFoundException(nameof(Group));
            group.CheckUploadContions();

            var request = new ChapterRequest
            {
                Chapter = chapter,
                Group = group,
                SubmitNote = data.SubmitNote,
                IsApproved = false,
                IsReviewed = false
            };

            await dbContext.Requests.AddAsync(request);
            await dbContext.SaveChangesAsync();
            return request;
        }

        public async Task<AuthorRequest> Submit(AuthorRequestData data)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var person = await dbContext.People.FirstOrDefaultAsync(p => p.Id == data.PersonId) ??
                throw new NotFoundException(nameof(Person));
            if (person.IsApproved)
            {
                throw new AlreadyApprovedException(nameof(Group));
            }
            var request = new AuthorRequest
            {
                Author = person,
                SubmitNote = data.SubmitNote,
                IsApproved = false,
                IsReviewed = false
            };
            await dbContext.Requests.AddAsync(request);
            await dbContext.SaveChangesAsync();
            return request;
        }

        public async Task<ArtistRequest> Submit(ArtistRequestData data)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var person = await dbContext.People.FirstOrDefaultAsync(p => p.Id == data.PersonId) ??
                throw new NotFoundException(nameof(Person));
            if (person.IsApproved)
            {
                throw new AlreadyApprovedException(nameof(Group));
            }
            var request = new ArtistRequest
            {
                Artist = person,
                SubmitNote = data.SubmitNote,
                IsApproved = false,
                IsReviewed = false
            };
            await dbContext.Requests.AddAsync(request);
            await dbContext.SaveChangesAsync();
            return request;
        }

        public async Task<Request> Update(string id, string note)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var request = await dbContext.Requests.FirstOrDefaultAsync(r => r.Id == id) ?? throw new NotFoundException(typeof(Request).Name);
            request.ReviewNote = note;
            await dbContext.SaveChangesAsync();
            return request;
        }

        public async Task<Request> Review(string id, string note, bool isApproved)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var request = await dbContext.Requests.Where(r => r.Id == id)
                .Include(r => (r as ChapterRequest).Chapter)
                .Include(r => (r as TitleRequest).Title)
                .Include(r => (r as MemberRequest).Member)
                .Include(r => (r as GroupRequest).Group)
                .Include(r => (r as ArtistRequest).Artist)
                .Include(r => (r as AuthorRequest).Author)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException(typeof(Request).Name);
            if (request.IsReviewed)
            {
                throw new AlreadyReviewedException();
            }

            request.ReviewNote = note;
            request.IsApproved = isApproved;
            request.IsReviewed = true;

            if (request.GetType() == typeof(ChapterRequest))
            {
                var rq = (ChapterRequest)request;
                rq.Chapter.IsApproved = isApproved;
            }
            else if (request.GetType() == typeof(TitleRequest))
            {
                var rq = (TitleRequest)request;
                if (!rq.Title.IsApproved)
                {
                    rq.Title.IsApproved = isApproved;
                }
            }
            else if (request.GetType() == typeof(GroupRequest))
            {
                var rq = (GroupRequest)request;
                rq.Group.IsApproved = isApproved;
            }
            else if (request.GetType() == typeof(MemberRequest))
            {
                var rq = (MemberRequest)request;
                rq.Member.IsApproved = isApproved;
            }
            else if (request.GetType() == typeof(ArtistRequest))
            {
                var rq = (ArtistRequest)request;
                rq.Artist.IsApproved = isApproved;
            }
            else if (request.GetType() == typeof(AuthorRequest))
            {
                var rq = (AuthorRequest)request;
                rq.Author.IsApproved = isApproved;
            }
            else
            {
                //TO BE FIXED
                throw new Exception();
            }

            await dbContext.SaveChangesAsync();
            return request;
        }

        public async Task<bool> Remove(string id)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var request = await dbContext.Requests.Where(r => r.Id == id).FirstOrDefaultAsync() ??
                throw new NotFoundException(nameof(Request));
            dbContext.Requests.Remove(request);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
