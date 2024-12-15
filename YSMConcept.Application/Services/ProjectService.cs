using Microsoft.AspNetCore.Http;
using YSMConcept.Domain.Entities;
using Microsoft.Extensions.Logging;
using YSMConcept.Application.Mappers;
using YSMConcept.Application.Services.Interfaces;
using YSMConcept.Application.DTOs.ProjectDTOs;
using YSMConcept.Application.Interfaces;

namespace YSMConcept.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageUploadService _imageUploadService;
        private readonly IImageDeleteService _imageDeleteService;
        private ILogger<ProjectService> _logger;

        public ProjectService(
            IUnitOfWork unitOfWork, 
            IImageUploadService imageUploadService,
            IImageDeleteService imageDeleteService,
            ILogger<ProjectService> logger)
        {
            _unitOfWork = unitOfWork;
            _imageUploadService = imageUploadService;
            _imageDeleteService = imageDeleteService;
            _logger = logger;
        }

        public async Task<ProjectDTO?> GetByIdAsync(Guid projectId)
        {
            var projectEntity = await _unitOfWork.Projects.GetByIdAsync(projectId);

            if (projectEntity != null)
            {
                return projectEntity.ToProjectDTOFromProjectEntity();
            }
            return null;
        }

        public async Task<List<ProjectDTO>> GetAllAsync(int pageNumber, int pageSize)
        {
            var projectEntities = await _unitOfWork.Projects.GetAllAsync(pageNumber, pageSize);

            var projectDtos = projectEntities.Select(project => project.ToProjectDTOFromProjectEntity()).ToList();
            return projectDtos;
        }

        public async Task<ProjectDTO> AddAsync(CreateProjectDTO createProjectDTO)
        {
            var projectEntity = createProjectDTO.ToProjectFromCreateProjectDTO();
            var imageEntities = await UploadAllImagesAsync(
                createProjectDTO.CollectionImages,
                createProjectDTO.MainImage,
                projectEntity.ProjectId);
            
            await _unitOfWork.BeginTransactionAsync(); 
            try
            {
                await _unitOfWork.Projects.AddAsync(projectEntity);
                foreach (var imageEntity in imageEntities)
                {
                    await _unitOfWork.Images.AddAsync(imageEntity);
                }
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return projectEntity.ToProjectDTOFromProjectEntity();
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "The transaction failed for projectId: {ProjectId}", projectEntity.ProjectId);

                await _imageDeleteService.DeleteImagesAsync(imageEntities);
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
        public async Task<ProjectDTO?> UpdateAsync(UpdateProjectDTO updateProjectDTO, Guid projectId)
        {
            var projectEntity = updateProjectDTO.ToProjectFromUpdateProjectDTO();
            var updatedProject = await _unitOfWork.Projects.UpdateAsync(projectEntity, projectId);

            if (updatedProject != null)
            {
                await _unitOfWork.SaveChangesAsync();
                return updatedProject.ToProjectDTOFromProjectEntity();
            }

            return null;
        }

        public async Task DeleteAsync(Guid projectId)
        {
            var imageEntities = await _unitOfWork.Images.GetAllByProjectIdAsync(projectId);

            await _imageDeleteService.DeleteImagesAsync(imageEntities);
            await _unitOfWork.Projects.DeleteAsync(projectId);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<List<ImageEntity>> UploadAllImagesAsync(List<IFormFile>? collectionImages, IFormFile? mainImage, Guid projectId)
        {
            List<ImageEntity> imageEntities = new();
            if(collectionImages != null)
            {
                imageEntities.AddRange(await _imageUploadService.UploadImagesAsync(collectionImages, projectId));
            }
            if(mainImage != null)
            {
                imageEntities.Add(await _imageUploadService.UploadImageAsync(mainImage, projectId));
            }
            return imageEntities;
        }
    }
}
