
using Microsoft.AspNetCore.Components.Forms;

namespace ClientLibrary.Services.Contracts;

public interface IFileService
{
    Task<string> UploadAvatarAsync(IBrowserFile file);
    Task<string> UploadAttachmentAsync(IBrowserFile file);
}
