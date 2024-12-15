using Microsoft.AspNetCore.Http;
using YSMConcept.Domain.Entities;

namespace YSMConcept.Application.Services.Interfaces
{
    public interface IImageUploadService
    {
        public Task<ImageEntity> UploadImageAsync(IFormFile image, Guid projectId);
        public Task<List<ImageEntity>> UploadImagesAsync(List<IFormFile> images, Guid projectId);
    }
}
