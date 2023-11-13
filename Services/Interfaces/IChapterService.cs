﻿using MangaHomeService.Models;
using MangaHomeService.Utils;

namespace MangaHomeService.Services.Interfaces
{
    public interface IChapterService
    {
        public Task<Chapter> Get(string id);
        public Task<List<Chapter>> GetByTitle (string titleId);
        public Task<Chapter> Add(double number, string titleId, string groupId, string? volumeId = null, string? languageId = null,
            List<string>? pagesIds = null, List<string>? commentsIds = null, bool isApproved = false);
        public Task<Chapter> Update(string id, double number = 0, string? titleId = null, string? groupId = null, string? volumeId = null,
            string? languageId = null, List<string>? pagesIds = null, List<string>? commentsIds = null, bool? isApproved = null);
        public Task<bool> Delete(string id);
        public Task<ChapterRequest> SubmitRequest(string titleId, string note, string groupId);
        public Task<ChapterRequest> GetRequest(string id);
        public Task<ChapterRequest> ReviewRequest(string requestId, string note, bool isApproved);
    }
}
