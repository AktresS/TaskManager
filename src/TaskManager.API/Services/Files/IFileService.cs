
namespace TaskManager.Services.Files;

public interface IFileService
{
    Task<string> UploadImageAsync(IFormFile file);
    Task<string> UploadFileAsync(IFormFile file);
}