using YSMConcept.Application.DTOs.ProjectDTOs;
using YSMConcept.Domain.Entities;

namespace YSMConcept.Application.Mappers
{
    public static class ProjectMapper
    {
        public static Project ToProjectFromCreateProjectDTO(this CreateProjectDTO createProjectDTO)
        {
            return new Project
            {
                Name = createProjectDTO.Name,
                Area = createProjectDTO.Area,
                Description = createProjectDTO.Description,
                BuildingType = createProjectDTO.BuildingType,
                Date = createProjectDTO.Date,
                Address = createProjectDTO.Address,
            };
        }
        public static ProjectDTO ToProjectDTOFromProjectEntity(this Project projectEntity)
        {
            return new ProjectDTO
            {
                ProjectId = projectEntity.ProjectId,
                Name = projectEntity.Name,
                Area = projectEntity.Area,
                Description = projectEntity.Description,
                BuildingType = projectEntity.BuildingType,
                Date = projectEntity.Date,
                Address = projectEntity.Address,
                CollectionImages = projectEntity.CollectionImages?.Select(image => image.ToImageDTOFromImageEntity()).ToList()
            };
        }
        public static Project ToProjectFromUpdateProjectDTO(this UpdateProjectDTO updateProjectDTO)
        {
            return new Project
            {
                Name = updateProjectDTO.Name,
                BuildingType = updateProjectDTO.BuildingType,
                Area = updateProjectDTO.Area,
                Address = updateProjectDTO.Address,
                Date = updateProjectDTO.Date,
                Description = updateProjectDTO.Description,
            };
        }
    }
}
