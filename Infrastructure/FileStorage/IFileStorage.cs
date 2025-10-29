using Microsoft.AspNetCore.Http;

namespace Infrastructure.FileStorage;

public interface IFileStorage
{
    Task<string> UploadFile(IFormFile file,string relativeFolder);
    Task DeleteFile(string? relativeFolder);
}