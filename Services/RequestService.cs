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
        public Task<ICollection<object>> GetAll(string keyword = "", int pageNumber = 1, int pageSize = Constants.RequestsPerPage, int? requestType = null, bool isReviewedIncluded = true);
        public Task<Request> Add(SubmitRequest submitRequest);
        public Task<Request> Update(string id, string note);
        public Task<Request> Review(string id, string note, bool isApproved);
        public Task<bool> Remove(string id);
    }

    public class RequestService(IDbContextFactory<MangaHomeDbContext> contextFactory) : IRequestService
    {
        public async Task<Request> Get(string id)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var request = await dbContext.Requests.Where(r => r.Id == id).FirstOrDefaultAsync()
                ?? throw new NotFoundException();
            if (request is GroupRequest groupRequest)
            {
                await dbContext.Entry(groupRequest).Reference(r => r.Group).LoadAsync();
            }
            else if (request is MemberRequest memberRequest)
            {
                await dbContext.Entry(memberRequest).Reference(r => r.Group).LoadAsync();
                await dbContext.Entry(memberRequest).Reference(r => r.Member).LoadAsync();
                await dbContext.Entry(memberRequest.Member).Reference(m => m.User).LoadAsync();
            }
            else if (request is TitleRequest titleRequest)
            {
                await dbContext.Entry(titleRequest).Reference(r => r.Title).LoadAsync();
                await dbContext.Entry(titleRequest).Reference(r => r.Group).LoadAsync();
            }
            else if (request is ChapterRequest chapterRequest)
            {
                await dbContext.Entry(chapterRequest).Reference(r => r.Chapter).LoadAsync();
                await dbContext.Entry(chapterRequest).Reference(r => r.Group).LoadAsync();
            }
            else if (request is AuthorRequest authorRequest)
            {
                await dbContext.Entry(authorRequest).Reference(r => r.Author).LoadAsync();
            }
            else if (request is ArtistRequest artistRequest)
            {
                await dbContext.Entry(artistRequest).Reference(r => r.Artist).LoadAsync();
            }
            else
            {
                //TO BE FIXED
                throw new Exception();
            }
            return request;
        }

        public async Task<ICollection<object>> GetAll(string keyword = "", int pageNumber = 1, int pageSize = Constants.RequestsPerPage, int? requestType = null, bool isReviewedIncluded = true)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            if (isReviewedIncluded)
            {
                if (requestType == (int)Enums.RequestType.Group)
                {
                    var results = await dbContext.Requests.OfType<GroupRequest>()
                        .Include(r => r.Group).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                    return (ICollection<object>)results.Where(r => r.Group.Name.Contains(keyword)).ToList();
                }
                else if (requestType == (int)Enums.RequestType.Member)
                {
                    return (ICollection<object>)await dbContext.Requests.OfType<MemberRequest>()
                        .Include(r => r.Group).Include(r => r.Member)
                        .Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (requestType == (int)Enums.RequestType.Title)
                {
                    return (ICollection<object>)await dbContext.Requests.OfType<TitleRequest>()
                        .Include(r => r.Group).Include(r => r.Title)
                        .Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (requestType == (int)Enums.RequestType.Chapter)
                {
                    return (ICollection<object>)await dbContext.Requests.OfType<ChapterRequest>()
                        .Include(r => r.Group).Include(r => r.Chapter)
                        .Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (requestType == (int)Enums.RequestType.Author)
                {
                    return (ICollection<object>)await dbContext.Requests.OfType<AuthorRequest>()
                        .Include(r => r.Author).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (requestType == (int)Enums.RequestType.Group)
                {
                    return (ICollection<object>)await dbContext.Requests.OfType<ArtistRequest>()
                        .Include(r => r.Artist).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (requestType == null)
                {
                    var requests = (ICollection<object>)await dbContext.Requests.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                    foreach (var request in requests)
                    {
                        if (request is GroupRequest groupRequest)
                        {
                            await dbContext.Entry(groupRequest).Reference(r => r.Group).LoadAsync();
                        }
                        else if (request is MemberRequest memberRequest)
                        {
                            await dbContext.Entry(memberRequest).Reference(r => r.Group).LoadAsync();
                            await dbContext.Entry(memberRequest).Reference(r => r.Member).LoadAsync();
                            await dbContext.Entry(memberRequest.Member).Reference(m => m.User).LoadAsync();
                        }
                        else if (request is TitleRequest titleRequest)
                        {
                            await dbContext.Entry(titleRequest).Reference(r => r.Title).LoadAsync();
                            await dbContext.Entry(titleRequest).Reference(r => r.Group).LoadAsync();
                        }
                        else if (request is ChapterRequest chapterRequest)
                        {
                            await dbContext.Entry(chapterRequest).Reference(r => r.Chapter).LoadAsync();
                            await dbContext.Entry(chapterRequest).Reference(r => r.Group).LoadAsync();
                        }
                        else if (request is AuthorRequest authorRequest)
                        {
                            await dbContext.Entry(authorRequest).Reference(r => r.Author).LoadAsync();
                        }
                        else if (request is ArtistRequest artistRequest)
                        {
                            await dbContext.Entry(artistRequest).Reference(r => r.Artist).LoadAsync();
                        }
                        else
                        {
                            //TO BE FIXED
                            throw new Exception();
                        }
                    }
                    return requests;
                }
                else
                {
                    //TO BE FIXED
                    throw new Exception();
                }
            }
            else
            {
                if (requestType == (int)Enums.RequestType.Group)
                {
                    return (ICollection<object>)await dbContext.Requests.OfType<GroupRequest>()
                        .Where(r => !r.IsReviewed)
                        .Include(r => r.Group)
                        .Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (requestType == (int)Enums.RequestType.Member)
                {
                    return (ICollection<object>)await dbContext.Requests.OfType<MemberRequest>()
                        .Where(r => !r.IsReviewed)
                        .Include(r => r.Group).Include(r => r.Member)
                        .Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (requestType == (int)Enums.RequestType.Title)
                {
                    return (ICollection<object>)await dbContext.Requests.OfType<TitleRequest>()
                        .Where(r => !r.IsReviewed)
                        .Include(r => r.Group).Include(r => r.Title)
                        .Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (requestType == (int)Enums.RequestType.Chapter)
                {
                    return (ICollection<object>)await dbContext.Requests.OfType<ChapterRequest>()
                        .Where(r => !r.IsReviewed)
                        .Include(r => r.Group).Include(r => r.Chapter)
                        .Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (requestType == (int)Enums.RequestType.Author)
                {
                    return (ICollection<object>)await dbContext.Requests.OfType<AuthorRequest>()
                        .Where(r => !r.IsReviewed)
                        .Include(r => r.Author)
                        .Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (requestType == (int)Enums.RequestType.Artist)
                {
                    return (ICollection<object>)await dbContext.Requests.OfType<ArtistRequest>()
                        .Where(r => !r.IsReviewed)
                        .Include(r => r.Artist)
                        .Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                }
                else if (requestType == null)
                {
                    var requests = (ICollection<object>)await dbContext.Requests.Where(r => !r.IsReviewed)
                        .Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                    foreach (var request in requests)
                    {
                        if (request is GroupRequest groupRequest)
                        {
                            await dbContext.Entry(groupRequest).Reference(r => r.Group).LoadAsync();
                        }
                        else if (request is MemberRequest memberRequest)
                        {
                            await dbContext.Entry(memberRequest).Reference(r => r.Group).LoadAsync();
                            await dbContext.Entry(memberRequest).Reference(r => r.Member).LoadAsync();
                            await dbContext.Entry(memberRequest.Member).Reference(m => m.User).LoadAsync();
                        }
                        else if (request is TitleRequest titleRequest)
                        {
                            await dbContext.Entry(titleRequest).Reference(r => r.Title).LoadAsync();
                            await dbContext.Entry(titleRequest).Reference(r => r.Group).LoadAsync();
                        }
                        else if (request is ChapterRequest chapterRequest)
                        {
                            await dbContext.Entry(chapterRequest).Reference(r => r.Chapter).LoadAsync();
                            await dbContext.Entry(chapterRequest).Reference(r => r.Group).LoadAsync();
                        }
                        else if (request is AuthorRequest authorRequest)
                        {
                            await dbContext.Entry(authorRequest).Reference(r => r.Author).LoadAsync();
                        }
                        else if (request is ArtistRequest artistRequest)
                        {
                            await dbContext.Entry(artistRequest).Reference(r => r.Artist).LoadAsync();
                        }
                        else
                        {
                            //TO BE FIXED
                            throw new Exception();
                        }
                    }
                    return requests;
                }
                else
                {
                    //TO BE FIXED
                    throw new Exception();
                }
            }
        }

        public async Task<Request> Add(SubmitRequest data)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            if (data is GroupRequestData groupRequestData)
            {
                var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == groupRequestData.GroupId) ??
                    throw new NotFoundException(nameof(Group));
                if (group.IsApproved)
                {
                    throw new AlreadyApprovedException(nameof(Group));
                }
                var request = new GroupRequest
                {
                    Group = group,
                    SubmitNote = groupRequestData.SubmitNote,
                    IsApproved = false,
                    IsReviewed = false
                };
                await dbContext.Requests.AddAsync(request);
                await dbContext.SaveChangesAsync();
                return request;
            }
            else if (data is MemberRequestData memberRequestData)
            {
                var member = await dbContext.Members.Where(t => t.Id == memberRequestData.MemberId && t.IsApproved == false).FirstOrDefaultAsync() ??
                    throw new NotFoundException(nameof(Member));
                var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == memberRequestData.GroupId) ??
                    throw new NotFoundException(nameof(Group));
                var request = new MemberRequest
                {
                    Member = member,
                    Group = group,
                    SubmitNote = memberRequestData.SubmitNote
                };

                await dbContext.Requests.AddAsync(request);
                await dbContext.SaveChangesAsync();
                return request;
            }
            else if (data is TitleRequestData titleRequestData)
            {
                var title = await dbContext.Titles.Where(t => t.Id == titleRequestData.TitleId && t.IsApproved == false).FirstOrDefaultAsync() ??
                    throw new NotFoundException(nameof(Title));
                var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == titleRequestData.GroupId) ??
                    throw new NotFoundException(nameof(Group));
                var request = new TitleRequest
                {
                    Title = title,
                    Group = group,
                    SubmitNote = titleRequestData.SubmitNote
                };

                await dbContext.Requests.AddAsync(request);
                await dbContext.SaveChangesAsync();
                return request;
            }
            else if (data is ChapterRequestData chapterRequestData)
            {
                var chapter = await dbContext.Chapters.FirstOrDefaultAsync(t => t.Id == chapterRequestData.ChapterId) ??
                    throw new NotFoundException(nameof(Chapter));
                if (chapter.IsApproved)
                {
                    throw new AlreadyApprovedException(nameof(Chapter));
                }
                var group = await dbContext.Groups.FirstOrDefaultAsync(g => g.Id == chapterRequestData.GroupId) ??
                    throw new NotFoundException(nameof(Group));
                group.CheckUploadContions();

                var request = new ChapterRequest
                {
                    Chapter = chapter,
                    Group = group,
                    SubmitNote = chapterRequestData.SubmitNote,
                    IsApproved = false,
                    IsReviewed = false
                };

                await dbContext.Requests.AddAsync(request);
                await dbContext.SaveChangesAsync();
                return request;
            }
            else if (data is AuthorRequestData authorRequestData)
            {
                var person = await dbContext.People.FirstOrDefaultAsync(p => p.Id == authorRequestData.PersonId) ??
                    throw new NotFoundException(nameof(Person));
                if (person.IsApproved)
                {
                    throw new AlreadyApprovedException(nameof(Group));
                }
                var request = new AuthorRequest
                {
                    Author = person,
                    SubmitNote = authorRequestData.SubmitNote,
                    IsApproved = false,
                    IsReviewed = false
                };
                await dbContext.Requests.AddAsync(request);
                await dbContext.SaveChangesAsync();
                return request;
            }
            else if (data is ArtistRequestData artistRequestData)
            {
                var person = await dbContext.People.FirstOrDefaultAsync(p => p.Id == artistRequestData.PersonId) ??
                    throw new NotFoundException(nameof(Person));
                if (person.IsApproved)
                {
                    throw new AlreadyApprovedException(nameof(Group));
                }
                var request = new ArtistRequest
                {
                    Artist = person,
                    SubmitNote = artistRequestData.SubmitNote,
                    IsApproved = false,
                    IsReviewed = false
                };
                await dbContext.Requests.AddAsync(request);
                await dbContext.SaveChangesAsync();
                return request;
            }
            else
            {
                //TO BE FIXED
                throw new Exception();
            }
        }

        public async Task<Request> Update(string id, string note)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var request = await dbContext.Requests.FirstOrDefaultAsync(r => r.Id == id)
                ?? throw new NotFoundException(typeof(Request).Name);
            request.SubmitNote = note;
            await dbContext.SaveChangesAsync();
            return request;
        }

        public async Task<Request> Review(string id, string note, bool isApproved)
        {
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var request = await dbContext.Requests.Where(r => r.Id == id).FirstOrDefaultAsync()
                ?? throw new NotFoundException();
            if (request is GroupRequest groupRequest)
            {
                await dbContext.Entry(groupRequest).Reference(r => r.Group).LoadAsync();
            }
            else if (request is MemberRequest memberRequest)
            {
                await dbContext.Entry(memberRequest).Reference(r => r.Member).LoadAsync();
            }
            else if (request is TitleRequest titleRequest)
            {
                await dbContext.Entry(titleRequest).Reference(r => r.Title).LoadAsync();
            }
            else if (request is ChapterRequest chapterRequest)
            {
                await dbContext.Entry(chapterRequest).Reference(r => r.Chapter).LoadAsync();
            }
            else if (request is AuthorRequest authorRequest)
            {
                await dbContext.Entry(authorRequest).Reference(r => r.Author).LoadAsync();
            }
            else if (request is ArtistRequest artistRequest)
            {
                await dbContext.Entry(artistRequest).Reference(r => r.Artist).LoadAsync();
            }
            else
            {
                //TO BE FIXED
                throw new Exception();
            }

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
            using var dbContext = await contextFactory.CreateDbContextAsync();
            var request = await dbContext.Requests.Where(r => r.Id == id).FirstOrDefaultAsync() ??
                throw new NotFoundException(nameof(Request));
            dbContext.Requests.Remove(request);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
