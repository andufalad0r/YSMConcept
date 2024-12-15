using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using YSMConcept.Application.Interfaces;

namespace YSMConcept.Infrastructure.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(
            Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<UploadResult> UploadFileAsync(IFormFile file)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream())
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult;
        }

        public async Task DeleteFileAsync(string fileId)
        {
            var deleteParams = new DelResParams()
            {
                PublicIds = new List<string> { fileId },
                Type = "upload",
                ResourceType = ResourceType.Image
            };
            var result = await _cloudinary.DeleteResourcesAsync(deleteParams);
        }

        public async Task DeleteFilesAsync(List<string> filesId)
        {
            var deleteParams = new DelResParams()
            {
                PublicIds = filesId,
                Type = "upload",
                ResourceType = ResourceType.Image
            };
            var result = await _cloudinary.DeleteResourcesAsync(deleteParams);
        }
    }
}
