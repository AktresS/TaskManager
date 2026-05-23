
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using TaskManager.Cloud;

namespace TaskManager.Services.Files;

public class FileService(IOptions<CloudinarySettings> config) : IFileService
{
    private readonly Cloudinary _cloudinary = new(new Account(
        config.Value.CloudName,
        config.Value.ApiKey,
        config.Value.ApiSecret));

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        if (file.Length == 0)
            throw new ArgumentException("File is empty");

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            throw new ArgumentException("Only images are allowed");

        if (file.Length > 5 * 1024 * 1024)
            throw new ArgumentException("File size must be less than 5MB");

        using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "taskmanager/avatars",
            Transformation = new Transformation()
                .Width(200).Height(200).Crop("fill").Gravity("face")
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error is not null)
            throw new Exception(result.Error.Message);

        return result.SecureUrl.ToString();
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        if (file.Length == 0)
            throw new ArgumentException("File is empty");

        if (file.Length > 10 * 1024 * 1024)
            throw new ArgumentException("File size must be less than 10MB");

        using var stream = file.OpenReadStream();

        var uploadParams = new RawUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "taskmanager/attachments"
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error is not null)
            throw new Exception(result.Error.Message);

        return result.SecureUrl.ToString();
    }
}
