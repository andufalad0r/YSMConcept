using Microsoft.AspNetCore.Http;
using YSMConcept.Domain.Entities;

namespace YSMConcept.Application.Services.Interfaces
{
    public interface IImageService
    {
        public Task<ImageEntity?> GetByIdAsync(string imageId);
        public Task<List<ImageEntity>> GetByProjectIdAsync(Guid projectId);
        public Task<List<ImageEntity>> AddAsync(List<IFormFile> images, Guid projectId);
        public Task<ImageEntity> AddMainAsync(IFormFile mainImage, Guid projectId);
        public Task DeleteAsync(List<string> imageIds);
    }
}
