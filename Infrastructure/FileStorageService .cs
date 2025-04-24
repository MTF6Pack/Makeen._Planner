using Application.Contracts;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Infrastructure
{
    public class FileStorageService : IFileStorageService
    {
        private const long MaxFileSizeInBytes = 1 * 1024 * 1024; // 1 MB
        private static readonly string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new BadRequestException("File is empty");

            if (file.Length > MaxFileSizeInBytes)
                throw new BadRequestException("File size is too large. Max allowed size is 1 MB.");

            try
            {
                Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                string extension = Path.GetExtension(file.FileName);
                string uniqueFileName = $"{Guid.NewGuid()}{extension}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 65536, true);
                await file.CopyToAsync(stream);

                return $"/uploads/{uniqueFileName}";
            }
            catch (IOException ex)
            {
                throw new BadRequestException($"File save failed: {ex.Message}");
            }
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            try
            {
                string filePath = ConvertUrlToPath(fileUrl);
                if (File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath));
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException($"Failed to delete file: {ex.Message}");
            }
        }

        private static string ConvertUrlToPath(string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl) || !fileUrl.StartsWith("/uploads/"))
                throw new BadRequestException("Invalid file URL format");

            return Path.Combine(uploadsFolder, Path.GetFileName(fileUrl));
        }
    }
}