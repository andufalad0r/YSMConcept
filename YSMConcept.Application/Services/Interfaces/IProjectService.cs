using YSMConcept.Application.DTOs.ProjectDTOs;
using YSMConcept.Domain.Entities;

namespace YSMConcept.Application.Services.Interfaces
{
    public interface IProjectService
    {
        public Task<ProjectDTO?> GetByIdAsync(Guid projectId);
        public Task<List<ProjectDTO>> GetAllAsync(int pageNumber, int pageSize);
        public Task<ProjectDTO> AddAsync(CreateProjectDTO createProjectDTO);
        public Task<ProjectDTO?> UpdateAsync(UpdateProjectDTO updatedProjectDTO, Guid projectId);
        public Task DeleteAsync(Guid projectId);
    }
}
