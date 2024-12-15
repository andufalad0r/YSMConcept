using Microsoft.AspNetCore.Http;
using YSMConcept.Application.Interfaces;
using YSMConcept.Application.Services.Interfaces;
using YSMConcept.Domain.Entities;

namespace YSMConcept.Infrastructure.Services
{
    public class ImageUploadService : IImageUploadService
    {
        private readonly ICloudinaryService _cloudinaryService;
        public ImageUploadService(
            ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ImageEntity> UploadImageAsync(IFormFile image, Guid projectId)
        {
            var uploadResult = await _cloudinaryService.UploadFileAsync(image);

            var imageEntity = new ImageEntity
            {
                ImageId = uploadResult.PublicId,
                ImageURL = uploadResult.Url.ToString(),
                ProjectId = projectId,
                IsMain = true,
            };

            return imageEntity;
        }

        public async Task<List<ImageEntity>> UploadImagesAsync(List<IFormFile> images, Guid projectId)
        {
            var uploadTasks = images.Select(image => _cloudinaryService.UploadFileAsync(image));
            var uploadResults = await Task.WhenAll(uploadTasks);
            return uploadResults
                .Where(result => result.StatusCode == System.Net.HttpStatusCode.OK)
                .Select(result => new ImageEntity
                {
                    ImageId = result.PublicId,
                    ImageURL = result.Url.ToString(),
                    ProjectId = projectId,
                })
                .ToList();
        }
    }
}
