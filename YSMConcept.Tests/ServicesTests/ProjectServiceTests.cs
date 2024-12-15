using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using YSMConcept.Application.DTOs.ProjectDTOs;
using YSMConcept.Application.Interfaces;
using YSMConcept.Application.Mappers;
using YSMConcept.Application.Services;
using YSMConcept.Application.Services.Interfaces;
using YSMConcept.Domain.Entities;
using YSMConcept.Domain.ValueObjects;
using YSMConcept.Infrastructure.Services;
using static System.Net.Mime.MediaTypeNames;

namespace YSMConcept.Tests.ServicesTests
{
    public class ProjectServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<IImageUploadService> _mockImageUploadService = new();
        private readonly Mock<IImageDeleteService> _mockImageDeleteService = new();
        private readonly Mock<ILogger<ProjectService>> _loggerMock = new();
        private readonly ProjectService _projectService;

        public ProjectServiceTests()
        {
            _projectService = new ProjectService(
                _mockUnitOfWork.Object,
                _mockImageUploadService.Object,
                _mockImageDeleteService.Object, 
                _loggerMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingProjectId_ReturnsProjectDto()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var projectEntity = new Project
            {
                ProjectId = projectId,
                Name = "Name",
                BuildingType = "BuildingType",
                Area = 56,
                Date = new Date(2004, 5),
                Address = new Address("City", "Street"),
                Description = "Description"
            };

            _mockUnitOfWork
                .Setup(uow => uow.Projects.GetByIdAsync(projectId))
                .ReturnsAsync(projectEntity);

            // Act
            var result = await _projectService.GetByIdAsync(projectId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(projectId, result.ProjectId);
            Assert.Equal("Name", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingProjectId_ReturnsNull()
        {
            // Arrange
            var projectId = Guid.NewGuid();

            _mockUnitOfWork
                .Setup(uow => uow.Projects.GetByIdAsync(projectId))
                .ReturnsAsync(() => null);

            // Act
            var result = await _projectService.GetByIdAsync(projectId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_TenElementsOfFirstPage_ReturnsListOfProjects()
        {
            // Arrange
            var projectId1 = Guid.NewGuid();
            var projectId2 = Guid.NewGuid();

            var testProjects = new List<Project>{
                new Project
                {
                    ProjectId = projectId1,
                    Name = "Name",
                    BuildingType = "BuildingType",
                    Area = 56,
                    Date = new Date(2004, 5),
                    Address = new Address("City", "Street"),
                    Description = "Description"
                },
                new Project
                {
                    ProjectId = projectId2,
                    Name = "Name",
                    BuildingType = "BuildingType",
                    Area = 56,
                    Date = new Date(2004, 5),
                    Address = new Address("City", "Street"),
                    Description = "Description"
                }
            };

            _mockUnitOfWork
                .Setup(uow => uow.Projects.GetAllAsync(0, 10))
                .ReturnsAsync(testProjects);

            // Act
            var result = await _projectService.GetAllAsync(0, 10);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.ProjectId == projectId1);
            Assert.Contains(result, r => r.ProjectId == projectId2);
        }

        [Fact]
        public async Task GetAllAsync_TenElementsOfFirstPage_ReturnsEmptyList()
        {
            // Arrange
            var emptyList = new List<Project>();

            _mockUnitOfWork
                .Setup(uow => uow.Projects.GetAllAsync(0, 10))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _projectService.GetAllAsync(0, 10);

            // Assert
            Assert.Empty(result);
        }

        private CreateProjectDTO GetValidDTO()
        {
            return new CreateProjectDTO
            {
                Name = "Valid Project",
                BuildingType = "Residential",
                Area = 15000,
                Date = new Date { Year = 2024, Month = 12 },
                Address = new Address { Street = "123 Main St", City = "Metropolis" },
                Description = "A valid project description.",
                MainImage = new FormFile(new MemoryStream(), 0, 0, null!, "main.jpg"),
                CollectionImages = new List<IFormFile>
                {
                    new FormFile(new MemoryStream(), 0, 0, null!, "image1.jpg"),
                    new FormFile(new MemoryStream(), 0, 0, null!, "image2.jpg")
                }
            };
        }

        [Fact]
        public async Task AddAsync_ValidCreateProjectDto_AddProjectWithImagesToDatabase()
        {
            // Arrange
            var createProjectDTO = GetValidDTO();
            var projectEntity = new Project { ProjectId = Guid.NewGuid() };
            var mainImage = new ImageEntity();
            var imageEntities = new List<ImageEntity> { new ImageEntity(), new ImageEntity() };

            _mockImageUploadService
                .Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<Guid>()))
                .ReturnsAsync(mainImage);

            _mockImageUploadService
                .Setup(x => x.UploadImagesAsync(It.IsAny<List<IFormFile>>(), It.IsAny<Guid>()))
                .ReturnsAsync(imageEntities);

            _mockUnitOfWork.Setup(x => x.Projects.AddAsync(It.IsAny<Project>()));
            _mockUnitOfWork.Setup(x => x.Images.AddAsync(It.IsAny<ImageEntity>()));
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.BeginTransactionAsync());
            _mockUnitOfWork.Setup(x => x.CommitTransactionAsync());

            // Act
            var result = await _projectService.AddAsync(createProjectDTO);

            // Assert
            _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _mockUnitOfWork.Verify(x => x.Projects.AddAsync(It.IsAny<Project>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Images.AddAsync(It.IsAny<ImageEntity>()), Times.Exactly(imageEntities.Count + 1));
            _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(), Times.Once);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task AddAsync_ValidCreateProjectDto_TransactionFails()
        {
            // Arrange
            var createProjectDTO = GetValidDTO();
            var projectEntity = new Project { ProjectId = Guid.NewGuid() };
            var imageEntities = new List<ImageEntity> { new ImageEntity(), new ImageEntity() };
            var mainImage = new ImageEntity();

            _mockImageUploadService
                .Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<Guid>()))
                .ReturnsAsync(mainImage);

            _mockImageUploadService
                .Setup(x => x.UploadImagesAsync(It.IsAny<List<IFormFile>>(), It.IsAny<Guid>()))
                .ReturnsAsync(imageEntities);


            _mockUnitOfWork.Setup(x => x.BeginTransactionAsync());
            _mockUnitOfWork.Setup(x => x.Projects.AddAsync(It.IsAny<Project>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _projectService.AddAsync(createProjectDTO));

            _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _mockUnitOfWork.Verify(x => x.RollbackTransactionAsync(), Times.Once);
            _mockImageDeleteService.Verify(x => x.DeleteImagesAsync(It.IsAny<List<ImageEntity>>()), Times.Once);
        }

        private UpdateProjectDTO GetValidUpdateDTO()
        {
            return new UpdateProjectDTO
            {
                Name = "Valid Project",
                BuildingType = "Residential",
                Area = 15000,
                Date = new Date { Year = 2024, Month = 12 },
                Address = new Address { Street = "123 Main St", City = "Metropolis" },
                Description = "A valid project description.",
            };
        }

        [Fact]
        public async Task UpdateAsync_ValidUpdateProjectDtoAndExistingProjectId_ReturnsProjectDto()
        {
            // Arrange
            var updateProjectDTO = GetValidUpdateDTO();
            var projectId = Guid.NewGuid();
            var projectEntity = updateProjectDTO.ToProjectFromUpdateProjectDTO();

            _mockUnitOfWork
                .Setup(x => x.Projects.UpdateAsync(It.IsAny<Project>(), projectId))
                .ReturnsAsync(projectEntity);

            _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _projectService.UpdateAsync(updateProjectDTO, projectId);

            // Assert 
            Assert.NotNull(result);
            Assert.Equal(result.Name, updateProjectDTO.Name);
        }

        [Fact]
        public async Task UpdateAsync_ValidUpdateProjectDtoAndNonExistingProjectId_ReturnsNull()
        {
            // Arrange
            var updateProjectDTO = GetValidUpdateDTO();
            var projectId = Guid.NewGuid();
            
            _mockUnitOfWork
                .Setup(x => x.Projects.UpdateAsync(updateProjectDTO.ToProjectFromUpdateProjectDTO(), projectId))
                .ReturnsAsync(() => null);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act

            var result = await _projectService.UpdateAsync(updateProjectDTO, projectId);

            // Assert 
            Assert.Null(result);
        }
    }
}
