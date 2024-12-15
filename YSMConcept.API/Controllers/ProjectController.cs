using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YSMConcept.API.Helpers;
using YSMConcept.Application.DTOs.ImageDTOs;
using YSMConcept.Application.DTOs.ProjectDTOs;
using YSMConcept.Application.Services.Interfaces;
using YSMConcept.Domain.Entities;

namespace YSMConcept.API.Controllers
{
    [Route("projects")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        public readonly IProjectService _projectService;
        public readonly IImageService _imageService;
        public readonly FileValidator _fileValidator;
        public readonly ILogger<ProjectController> _logger;

        public ProjectController(
            IProjectService projectService,
            IImageService imageService,
            FileValidator fileValidator,
            ILogger<ProjectController> logger)
        {
            _projectService = projectService;
            _imageService = imageService;
            _fileValidator = fileValidator;
            _logger = logger;
        }

        [HttpGet("{projectId:Guid}")]
        public async Task<ActionResult<ProjectDTO>> GetById([FromRoute] Guid projectId)
        {
            var projectDto = await _projectService.GetByIdAsync(projectId);
            if (projectDto == null)
            {
                return NotFound();
            }
            return projectDto;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProjectDTO>>> GetAllAsync([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var projectDtos = await _projectService.GetAllAsync(pageNumber, pageSize);
            return projectDtos;
        }

        [HttpGet("{projectId:Guid}/images")]
        public async Task<ActionResult<List<ImageEntity>>> GetImagesByProjectIdAsync([FromRoute] Guid projectId)
        {
            var imageEntities = await _imageService.GetByProjectIdAsync(projectId);
            if (imageEntities.Count == 0)
            {
                return NotFound();
            }
            return imageEntities;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ProjectDTO>> AddAsync([FromForm] CreateProjectDTO createProjectDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var mainImageError = _fileValidator.ValidateFile(createProjectDTO.MainImage);
            if (mainImageError != null)
                return BadRequest(mainImageError);

            var collectionErrors = _fileValidator.ValidateFiles(createProjectDTO.CollectionImages);
            if (collectionErrors.Any())
                return BadRequest(new { Messages = collectionErrors });

            var projectEntity = await _projectService.AddAsync(createProjectDTO);

            return CreatedAtAction(nameof(GetById), new { projectId = projectEntity.ProjectId }, projectEntity);
        }

        [Authorize]
        [HttpPost("{projectId:Guid}/images")]
        public async Task<ActionResult<List<ImageEntity>>> AddImagesAsync([FromRoute] Guid projectId, [FromForm] List<IFormFile> images)
        {
            if (images == null || !images.Any())
                return BadRequest("Image list cannot be empty.");

            var collectionErrors = _fileValidator.ValidateFiles(images);
            if (collectionErrors.Any())
                return BadRequest(new { Messages = collectionErrors });

            var imageEntities = await _imageService.AddAsync(images, projectId);

            return imageEntities;
        }

        [Authorize]
        [HttpPost("{projectId:Guid}/images/mainImage")]
        public async Task<ActionResult<ImageEntity>> AddMainImageAsync([FromRoute] Guid projectId, [FromForm] AddMainImageDTO addMainImageDTO)
        {
            var mainImageError = _fileValidator.ValidateFile(addMainImageDTO.MainImage);
            if (mainImageError != null)
                return BadRequest(mainImageError);

            var imageEntity = await _imageService.AddMainAsync(addMainImageDTO.MainImage, projectId);

            return imageEntity;
        }

        [Authorize]
        [HttpPut("{projectId:Guid}")]
        public async Task<ActionResult<ProjectDTO>> UpdateAsync([FromRoute] Guid projectId, [FromBody] UpdateProjectDTO updateProjectDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var updatedProject = await _projectService.UpdateAsync(updateProjectDTO, projectId);

            if (updatedProject == null)
            {
                return NotFound();
            }
            return updatedProject;
        }
        [Authorize]
        [HttpDelete("{projectId:Guid}")]
        public async Task<ActionResult> DeleteAsync([FromRoute] Guid projectId)
        {
            await _projectService.DeleteAsync(projectId);

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{projectId:Guid}/images")]
        public async Task<ActionResult> DeleteImagesAsync([FromBody] DeleteImagesDTO deleteImagesDTO)
        {
            await _imageService.DeleteAsync(deleteImagesDTO.ImageIds);

            return NoContent();
        }
    }
}
