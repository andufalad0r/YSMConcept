using Microsoft.AspNetCore.Http;
using YSMConcept.Application.Interfaces;
using YSMConcept.Application.Services.Interfaces;
using YSMConcept.Domain.Entities;

namespace YSMConcept.Application.Services
{
    public class ImageService : IImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageUploadService _imageUploadService;
        private readonly IImageDeleteService _imageDeleteService;

        public ImageService(
            IUnitOfWork unitOfWork,
            IImageUploadService imageUploadService,
            IImageDeleteService imageDeleteService)
        {
            _unitOfWork = unitOfWork;
            _imageUploadService = imageUploadService;
            _imageDeleteService = imageDeleteService;
        }

        public async Task<ImageEntity?> GetByIdAsync(string imageId)
        {
            var imageEntity = await _unitOfWork.Images.GetByIdAsync(imageId);
            
            return imageEntity;
        }

        public async Task<List<ImageEntity>> GetByProjectIdAsync(Guid projectId)
        {
            var imageEntities = await _unitOfWork.Images.GetAllByProjectIdAsync(projectId);

            return imageEntities;
        }

        public async Task<List<ImageEntity>> AddAsync(List<IFormFile> images, Guid projectId)
        {
            var imageEntities = await _imageUploadService.UploadImagesAsync(images, projectId);

            foreach (var imageEntity in imageEntities)
            {
                await _unitOfWork.Images.AddAsync(imageEntity);
            }
            await _unitOfWork.SaveChangesAsync();

            return imageEntities;
        }

        public async Task<ImageEntity> AddMainAsync(IFormFile mainImage, Guid projectId)
        {
            var mainImageEntity = await _unitOfWork.Images.GetMainAsync(projectId);
            if(mainImageEntity != null)
            {
                await _imageDeleteService.DeleteImageAsync(mainImageEntity.ImageId);
                await _unitOfWork.Images.DeleteAsync(mainImageEntity.ImageId);
            }
            var newMainImageEntity = await _imageUploadService.UploadImageAsync(mainImage, projectId);

            await _unitOfWork.Images.AddAsync(newMainImageEntity);
            await _unitOfWork.SaveChangesAsync();

            return newMainImageEntity;
        }

        public async Task DeleteAsync(List<string> imageIds)
        {
            await _imageDeleteService.DeleteImagesAsync(imageIds);
            foreach (var imageId in imageIds)
            {
                await _unitOfWork.Images.DeleteAsync(imageId);
            }
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
