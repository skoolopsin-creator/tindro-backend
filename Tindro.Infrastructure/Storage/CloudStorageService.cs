using Microsoft.AspNetCore.Http;
using Tindro.Application.Common.Interfaces;

namespace Tindro.Infrastructure.Storage;

public class CloudStorageService : IFileStorage
{
    public async Task<string> UploadAsync(IFormFile file, string folder)
    {
        var fileName = $"{folder}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        // TODO: upload to S3 / R2 / Azure Blob

        return $"https://cdn.tindro.com/{fileName}";
    }
}
