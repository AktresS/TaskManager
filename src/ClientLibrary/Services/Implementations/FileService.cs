
using System.Text.Json;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;
using Microsoft.AspNetCore.Components.Forms;

namespace ClientLibrary.Services.Implementations;

public class FileService(GetHttpClient getHttpClient) : IFileService
{
    private const string BaseUrl = "api/files";

    public async Task<string> UploadAvatarAsync(IBrowserFile file)
    {
        var client = await getHttpClient.GetPrivateHttpClient();

        using var content = new MultipartFormDataContent();
        using var stream = file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024);
        using var streamContent = new StreamContent(stream);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        content.Add(streamContent, "file", file.Name);

        var result = await client.PostAsync($"{BaseUrl}/avatar", content);
        var body = await result.Content.ReadAsStringAsync();

        if (!result.IsSuccessStatusCode)
            throw new Exception(body);

        var json = JsonSerializer.Deserialize<JsonElement>(body);
        return json.GetProperty("url").GetString()!;
    }

    public async Task<string> UploadAttachmentAsync(IBrowserFile file)
    {
        var client = await getHttpClient.GetPrivateHttpClient();

        using var content = new MultipartFormDataContent();
        using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
        using var streamContent = new StreamContent(stream);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        content.Add(streamContent, "file", file.Name);

        var result = await client.PostAsync($"{BaseUrl}/attachment", content);
        var body = await result.Content.ReadAsStringAsync();

        if (!result.IsSuccessStatusCode)
            throw new Exception(body);

        var json = JsonSerializer.Deserialize<JsonElement>(body);
        return json.GetProperty("url").GetString()!;
    }
}
