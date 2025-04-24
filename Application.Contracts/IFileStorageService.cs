using Microsoft.AspNetCore.Http;

namespace Application.Contracts
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task DeleteFileAsync(string fileUrl);
    }
}