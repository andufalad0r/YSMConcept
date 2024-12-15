using YSMConcept.Domain.Entities;

namespace YSMConcept.Domain.RepositoryInterfaces
{
    public interface IProjectRepository 
    {
        public Task<Project?> GetByIdAsync(Guid projectId);
        public Task<List<Project>> GetAllAsync(int pageNumber, int pageSize);
        public Task AddAsync(Project projectEntity);
        public Task DeleteAsync(Guid projectId);
        public Task<Project?> UpdateAsync(Project updatedProject, Guid projectId);
    }
}
