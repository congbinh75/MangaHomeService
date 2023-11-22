namespace MangaHomeService.Utils
{
    public static class Functions
    {
        public static async Task<string> UploadFileAsync(IFormFile file, string? path)
        {
            ArgumentNullException.ThrowIfNull(file);
            ArgumentNullException.ThrowIfNull(path);

            var filePath = Path.Combine(path, Path.GetRandomFileName());
            using var stream = File.OpenRead(filePath);
            await file.CopyToAsync(stream);
            return filePath;
        }
    }
}
