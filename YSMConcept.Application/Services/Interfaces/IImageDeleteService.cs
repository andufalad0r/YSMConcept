using YSMConcept.Domain.Entities;

namespace YSMConcept.Application.Services.Interfaces
{
    public interface IImageDeleteService
    {
        public Task DeleteImageAsync(string imageId);
        public Task DeleteImagesAsync(List<ImageEntity> imageEntities);
        public Task DeleteImagesAsync(List<string> imageIds);
    }
}
