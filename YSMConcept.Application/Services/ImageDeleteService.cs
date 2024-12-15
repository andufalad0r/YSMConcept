using Microsoft.Extensions.Logging;
using YSMConcept.Application.Interfaces;
using YSMConcept.Application.Services.Interfaces;
using YSMConcept.Domain.Entities;

namespace YSMConcept.Application.Services
{
    public class ImageDeleteService : IImageDeleteService
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<ImageDeleteService> _logger;

        public ImageDeleteService(
            ICloudinaryService cloudinaryService,
            ILogger<ImageDeleteService> logger)
        {
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        public async Task DeleteImageAsync(string imageId)
        {
            await _cloudinaryService.DeleteFileAsync(imageId);
        }

        public async Task DeleteImagesAsync(List<ImageEntity> imageEntities)
        {
            List<string> imageIds = imageEntities.Select(image => image.ImageId).ToList();
            await DeleteImagesAsync(imageIds);
        }

        public async Task DeleteImagesAsync(List<string> imageIds)
        {
            await _cloudinaryService.DeleteFilesAsync(imageIds);
        }
    }
}
