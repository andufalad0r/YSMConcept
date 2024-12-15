using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using YSMConcept.Domain.Entities;
using YSMConcept.Domain.RepositoryInterfaces;
using YSMConcept.Infrastructure.Data;

namespace YSMConcept.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly YsmDbContext _dbContext;
        private readonly ILogger<ProjectRepository> _logger;

        public ProjectRepository(
            YsmDbContext dbContext, 
            ILogger<ProjectRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Project?> GetByIdAsync(Guid projectId)
        {
            var projectEntity = await _dbContext.Projects
                .AsNoTracking()
                .Include(p => p.CollectionImages)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if(projectEntity == null)
                _logger.LogWarning($"Project record with ID {projectId} is missing.", projectId);
            
            return projectEntity;
        }

        public async Task<List<Project>> GetAllAsync(int pageNumber, int pageSize)
        {
            int skipPages = pageNumber * pageSize;

            var projectsEntities = await _dbContext.Projects
                .AsNoTracking()
                .Include(p => p.CollectionImages)
                .Skip(skipPages)
                .Take(pageSize)
                .ToListAsync();

            if(projectsEntities.Count == 0)
                _logger.LogWarning("The Projects table is empty. No records found.");
            
            return projectsEntities;
        }

        public async Task AddAsync(Project projectEntity)
        {
            await _dbContext.AddAsync(projectEntity);
            _logger.LogInformation($"New Project record with ID {projectEntity.ProjectId} was added to the table.", projectEntity);
        }

        public async Task<Project?> UpdateAsync(Project updatedProject, Guid projectId)
        {
            var projectEntity = await _dbContext.Projects.FirstOrDefaultAsync(project => project.ProjectId == projectId);
            if(projectEntity != null)
            {
                projectEntity.Name = updatedProject.Name;
                projectEntity.Description = updatedProject.Description;
                projectEntity.Area = updatedProject.Area;
                projectEntity.Address = updatedProject.Address;
                projectEntity.Date = updatedProject.Date;

                _logger.LogInformation($"New Project record with ID {projectEntity.ProjectId} was succesfully updated.", projectEntity);
                return projectEntity;
            }
            _logger.LogWarning($"Project with ID {projectId} wasn't found.", projectId);

            return projectEntity;
        }

        public async Task DeleteAsync(Guid projectId)
        {
            var projectEntity = await _dbContext.Projects.FindAsync(projectId);

            if (projectEntity != null)
            {
                _dbContext.Projects.Remove(projectEntity);
                _logger.LogInformation($"Project record with ID {projectEntity.ProjectId} was succesfully deleted.", projectEntity);
                return;
            }
            _logger.LogWarning($"Project with ID {projectId} wasn't found.", projectId);
        }
    }
}
