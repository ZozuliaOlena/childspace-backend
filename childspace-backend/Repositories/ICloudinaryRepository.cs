using Microsoft.AspNetCore.Http;
using childspace_backend.Models.DTOs;

namespace childspace_backend.Repositories
{
    public interface ICloudinaryRepository
    {
        Task<FileUploadResult> UploadAsync(IFormFile file);

        Task<bool> DeleteAsync(string publicId);
    }
}