using YSMConcept.Domain.Entities;

namespace YSMConcept.Domain.RepositoryInterfaces
{
    public interface IImageRepository
    {
        public Task<ImageEntity?> GetByIdAsync(string imageId);
        public Task<List<ImageEntity>> GetAllAsync(int pageNumber, int pageSize);
        public Task<List<ImageEntity>> GetAllByProjectIdAsync(Guid projectId);
        public Task<ImageEntity?> GetMainAsync(Guid projectId);
        public Task AddAsync(ImageEntity imageEntity);
        public Task DeleteAsync(string imageId);
    }
}
