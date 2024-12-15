using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YSMConcept.Application.DTOs.ProjectDTOs;
using YSMConcept.Application.Mappers;
using YSMConcept.Domain.Entities;
using YSMConcept.Domain.ValueObjects;
using YSMConcept.Infrastructure.Data;
using YSMConcept.Infrastructure.Repositories;

namespace YSMConcept.Tests.RepositoryTests
{
    public class ProjectRepositoryTests
    {
        private readonly DbContextOptions<YsmDbContext> _dbContextOptions;
        private readonly Mock<ILogger<ProjectRepository>> _loggerMock;

        public ProjectRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<YsmDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB for each test
                .Options;

            _loggerMock = new Mock<ILogger<ProjectRepository>>();
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnProject()
        {
            // Arrange
            using var context = new YsmDbContext(_dbContextOptions);

            var repository = new ProjectRepository(context, _loggerMock.Object);
            var projectId = Guid.NewGuid();
            var testProject = new Project
            {
                ProjectId = projectId,
                Name = "Name",
                BuildingType = "BuildingType",
                Area = 56,
                Date = new Date(2004, 5),
                Address = new Address("City", "Street"),
                Description = "Description"
            };
            context.Projects.Add(testProject);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(projectId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testProject.ProjectId, result.ProjectId);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsNullAndLogWarning()
        {
            // Arrange
            using var context = new YsmDbContext(_dbContextOptions);

            var repository = new ProjectRepository(context, _loggerMock.Object);
            var projectId = Guid.NewGuid();

            // Act
            var result = await repository.GetByIdAsync(projectId);

            // Assert
            Assert.Null(result);
            _loggerMock.VerifyLog(logger => logger.LogWarning($"Project record with ID {projectId} is missing.", projectId));
        }

        [Fact]
        public async Task GetAllAsync_TenElementsOfFirstPage_ReturnsAllProjects()
        {
            // Arrange
            using var context = new YsmDbContext(_dbContextOptions);

            var repository = new ProjectRepository(context, _loggerMock.Object);
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
            context.Projects.AddRange(testProjects);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAllAsync(0, 10);

            // Assert
            Assert.Equal(2, result.Count); 
            Assert.Contains(result, r => r.ProjectId == projectId1);
            Assert.Contains(result, r => r.ProjectId == projectId2);
        }

        [Fact]
        public async Task GetAllAsync_TenElementsOfFirstPage_ReturnsEmptyListAndLogWarning()
        {
            // Arrange
            using var context = new YsmDbContext(_dbContextOptions);

            var repository = new ProjectRepository(context, _loggerMock.Object);
            var projectId1 = Guid.NewGuid();
            var projectId2 = Guid.NewGuid();

            // Act
            var result = await repository.GetAllAsync(0, 10);

            // Assert
            Assert.Empty(result); 
            _loggerMock.VerifyLog(logger => logger.LogWarning("The Projects table is empty. No records found."));
        }

        [Fact]
        public async Task AddAsync_ProjectEntity_AddsProjectToDatabase()
        {
            // Arrange
            using var context = new YsmDbContext(_dbContextOptions);
            var repository = new ProjectRepository(context, _loggerMock.Object);

            var projectId = Guid.NewGuid();
            var testProject = new Project
            {
                ProjectId = projectId,
                Name = "Name",
                BuildingType = "Building Type",
                Area = 56,
                Date = new Date(2004, 5),
                Address = new Address("City", "Street"),
                Description = "Description"
            };

            // Act
            await repository.AddAsync(testProject);
            await context.SaveChangesAsync();

            // Assert
            var projectInDb = await context.Projects.FirstOrDefaultAsync(i => i.ProjectId == projectId);
            Assert.NotNull(projectInDb);
            Assert.Equal(projectId, projectInDb.ProjectId);
            _loggerMock.VerifyLog(logger => logger.LogInformation($"New Project record with ID {projectId} was added to the table.", projectId));
        }

        [Fact]
        public async Task UpdateAsync_ProjectEntityAndExistingProjectId_UpdatesProjectInDatabaseAndLogInformation()
        {
            // Arrange
            using var context = new YsmDbContext(_dbContextOptions);
            var repository = new ProjectRepository(context, _loggerMock.Object);

            var projectId = Guid.NewGuid();
            var testProject = new Project
            {
                ProjectId = projectId,
                Name = "Name",
                BuildingType = "Building Type",
                Area = 56,
                Date = new Date(2004, 5),
                Address = new Address("City", "Street"),
                Description = "Description"
            };

            var updatedProject = new UpdateProjectDTO
            {
                Name = "UpdatedName",
                BuildingType = "Building Type",
                Area = 56,
                Date = new Date(2004, 5),
                Address = new Address("City", "Street"),
                Description = "Description"
            };
            var updateEntity = updatedProject.ToProjectFromUpdateProjectDTO();

            // Act
            await repository.AddAsync(testProject);
            await context.SaveChangesAsync();
            await repository.UpdateAsync(updateEntity, testProject.ProjectId);
            await context.SaveChangesAsync();

            // Assert
            var projectInDb = await context.Projects.FirstOrDefaultAsync(i => i.ProjectId == projectId);
            Assert.NotNull(projectInDb);
            Assert.Equal(projectId, projectInDb.ProjectId);
            Assert.Equal("UpdatedName", projectInDb.Name);
            _loggerMock.VerifyLog(logger => logger.LogInformation($"New Project record with ID {projectId} was succesfully updated.", projectId));
        }
        [Fact]
        public async Task UpdateAsync_ProjectEntityAndNonExistingProjectId_ReturnsNullAndLogsWarning()
        {
            // Arrange
            using var context = new YsmDbContext(_dbContextOptions);
            var repository = new ProjectRepository(context, _loggerMock.Object);
            var projectId = Guid.NewGuid();
            var updateProject = new UpdateProjectDTO
            {
                Name = "UpdatedName",
                BuildingType = "Building Type",
                Area = 56,
                Date = new Date(2004, 5),
                Address = new Address("City", "Street"),
                Description = "Description"
            };
            var updateEntity = updateProject.ToProjectFromUpdateProjectDTO();

            // Act
            var updatedProject = await repository.UpdateAsync(updateEntity, projectId);
            await context.SaveChangesAsync();

            // Assert
            Assert.Null(updatedProject);
            _loggerMock.VerifyLog(logger => logger.LogWarning($"Project with ID {projectId} wasn't found.", projectId));
        }

        [Fact]
        public async Task DeleteAsync_ProjectId_DeletesProjectInDatabase()
        {
            // Arrange
            using var context = new YsmDbContext(_dbContextOptions);
            var repository = new ProjectRepository(context, _loggerMock.Object);

            var projectId = Guid.NewGuid();
            var testProject = new Project
            {
                ProjectId = projectId,
                Name = "Name",
                BuildingType = "Building Type",
                Area = 56,
                Date = new Date(2004, 5),
                Address = new Address("City", "Street"),
                Description = "Description"
            };
            await context.Projects.AddAsync(testProject);
            await context.SaveChangesAsync();
            // Act
            await repository.DeleteAsync(projectId);
            await context.SaveChangesAsync();

            // Assert
            var projectInDb = await context.Projects.FirstOrDefaultAsync(i => i.ProjectId == projectId);
            Assert.Null(projectInDb);
            _loggerMock.VerifyLog(logger => logger.LogInformation($"Project record with ID {projectId} was succesfully deleted.", projectId));
        }
        [Fact]
        public async Task DeleteAsync_ProjectId_ShouldLogWarning()
        {
            // Arrange
            using var context = new YsmDbContext(_dbContextOptions);
            var repository = new ProjectRepository(context, _loggerMock.Object);

            var projectId = Guid.NewGuid();
            
            // Act
            await repository.DeleteAsync(projectId);
            await context.SaveChangesAsync();

            // Assert
            _loggerMock.VerifyLog(logger => logger.LogWarning($"Project with ID {projectId} wasn't found.", projectId));
        }
    }
}
