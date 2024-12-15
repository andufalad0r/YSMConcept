using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace YSMConcept.Application.Interfaces
{
    public interface ICloudinaryService
    {
        public Task<UploadResult> UploadFileAsync(IFormFile file);
        public Task DeleteFileAsync(string file);
        public Task DeleteFilesAsync(List<string> filesId);
    }
}
