using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class IFormFileToUrl
    {
        private const long MaxFileSizeInBytes = 1 * 1024 * 1024; // 1 MB

        public static async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new BadRequestException("File is empty");

            if (file.Length > MaxFileSizeInBytes)
                throw new BadRequestException("File size is too large. Max allowed size is 1 MB.");

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

            string extension = Path.GetExtension(file.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}"; // Ensure unique file names
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            try
            {
                using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 65536, true);
                await file.CopyToAsync(stream);
            }
            catch (IOException ex)
            {
                throw new BadRequestException($"File save failed: {ex.Message}");
            }

            return $"/uploads/{uniqueFileName}"; // Return relative URL
        }

        public static async Task DeleteFileAsync(string fileUrl)
        {
            try
            {
                string filePath = ConvertUrlToPath(fileUrl);
                if (File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath)); // Async delete
                    Console.WriteLine($"Deleted file: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete file: {ex.Message}");
            }
        }

        private static string ConvertUrlToPath(string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl) || !fileUrl.StartsWith("/uploads/"))
                throw new ArgumentException("Invalid file URL format");

            string baseUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            string fileName = Path.GetFileName(fileUrl);
            return Path.Combine(baseUploadPath, fileName);
        }
    }
}