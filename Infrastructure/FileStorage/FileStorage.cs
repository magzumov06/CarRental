using Microsoft.AspNetCore.Http;

namespace Infrastructure.FileStorage;

public class FileStorage(string rootPath) : IFileStorage
{
    public async Task<string> UploadFile(IFormFile file, string relativeFolder)
    {
        try
        {
            var path = Path.Combine(rootPath , "wwwroot" , relativeFolder);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(path, fileName);
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return Path.Combine(relativeFolder, fileName).Replace("\\", "/");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw new Exception("Something went wrong");
        }
    }

    public Task DeleteFile(string? relativeFolder)
    {
        try
        {
            var path = Path.Combine(rootPath,"wwwroot", relativeFolder.Replace("/",Path.DirectorySeparatorChar.ToString()));
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw new  Exception("Error in delete file");
        }
    }
}