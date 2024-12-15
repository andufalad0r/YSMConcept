using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using YSMConcept.Domain.Entities;
using YSMConcept.Infrastructure.Data;
using YSMConcept.Infrastructure.Repositories;

namespace YSMConcept.Tests.RepositoryTests
{
    public class ImageRepositoryTests
    {
        private readonly DbContextOptions<YsmDbContext> _dbContextOptions;
        private readonly Mock<ILogger<ImageRepository>> _loggerMock;

        public ImageRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<YsmDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _loggerMock = new Mock<ILogger<ImageRepository>>();
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsImage()
        {
            // Arrange
            using var context = new YsmDbContext(_dbContextOptions);

            var repository = new ImageRepository(context, _loggerMock.Object);
            var testImage = new ImageEntity
            {
                ImageId = "ImageId",
                ProjectId = Guid.NewGuid(),
                ImageURL = "ImageURL"
            };
            context.Images.Add(testImage);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync("ImageId");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ImageId", result.ImageId);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsNullAndLogsWarning()
        {
            // Arrange
            using var context = new YsmDbContext(_dbContextOptions);
            var _loggerMock = new Mock<ILogger<ImageRepository>>();
            var repository = new ImageRepository(context, _loggerMock.Object);

            // Act
            var result = await repository.GetByIdAsync("ImageId");

            // Assert
            Assert.Null(result);
            _loggerMock.VerifyLog(logger => logger.LogWarning("Image with ID ImageId not found."));
        }

        [Fact]
        public async Task GetAllAsync_TenElementsOfFirstPage_ReturnsAllImages()
        {
            // Arrange
            using var context = new YsmDbContext(_dbContextOptions);
            var repository = new ImageRepository(context, _loggerMock.Object);

            context.Images.AddRange(
                new ImageEntity { ImageId = "ImageId1", ImageURL = "ImageURL", ProjectId = Guid.NewGuid() },
                new ImageEntity { ImageId = "ImageId2", ImageURL = "ImageURL", ProjectId = Guid.NewGuid() },
                new ImageEntity { ImageId = "ImageId3", ImageURL = "ImageURL", ProjectId = Guid.NewGuid() }
            );
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAllAsync(0, 10);

            // Assert
            Assert.Equal(3, result.Count); 
            Assert.Contains(result, r => r.ImageId == "ImageId1");
            Assert.Contains(result, r => r.ImageId == "ImageId2");
        }

        [Fact]
        public async Task GetAllAsync_TenElementsOfFirstPage_ReturnsEmptyList()
        {
            // Arrange
            using var context = new YsmDbContext(_dbContextOptions);
            var _loggerMock = new Mock<ILogger<ImageRepository>>();

            var repository = new ImageRepository(context, _loggerMock.Object);

            // Act
            var result = await repository.GetAllAsync(0, 10);

            // Assert
            Assert.Equal(result, new List<ImageEntity>() { });
            _loggerMock.VerifyLog(logger => logger.LogWarning("No images found in the database."));
        }

        [Fact]
        public async Task GetAllByProjectIdAsync_ExistingProjectId_ReturnsAllImagesRelatedToProject()
        {
            // Arrange
            using var context = new YsmDbContext(_dbContextOptions);
            var repository = new ImageRepository(context, _loggerMock.Object);
            var projectId = Guid.NewGuid();

            context.Images.AddRange(
                new ImageEntity { ImageId = "ImageId1", ImageURL = "ImageURL", ProjectId = projectId },
                new ImageEntity { ImageId = "ImageId2", ImageURL = "ImageURL", ProjectId = projectId },
                new ImageEntity { ImageId = "ImageId3", ImageURL = "ImageURL", ProjectId = Guid.NewGuid() } // Different project
            );
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAllByProjectIdAsync(projectId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, r => Assert.Equal(projectId, r.ProjectId));
        }

        [Fact]
        public async Task GetAllByProjectIdAsync_ExistingProjectId_ReturnsEmptyListOfImagesAndLogsWarning()
        {
            // Arrange
            using var context = new YsmDbContext(_dbContextOptions);
            var repository = new ImageRepository(context, _loggerMock.Object);
            var projectId = Guid.NewGuid();

            // Act
            var result = await repository.GetAllByProjectIdAsync(projectId);

            // Assert
            Assert.Equal(result, new List<ImageEntity>() { });
            _loggerMock.VerifyLog(logger => logger.LogWarning($"No images found for project with Id {projectId} in the database.", projectId));
        }

        [Fact]
        public async Task GetMainAsync_ExistingProjectId_ReturnsMainImageRelatedToProject()
        {
            // Arrange
            using var context = new YsmDbContext(_dbContextOptions);
            var repository = new ImageRepository(context, _loggerMock.Object);
            var projectId = Guid.NewGuid();
            var testImage = new ImageEntity
            {
                ImageId = "ImageId1",
                ImageURL = "ImageURL",
                ProjectId = projectId,
                IsMain = true
            };
            context.Images.Add(testImage);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetMainAsync(projectId);

            // Assert
            Assert.Equal(testImage, result);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetMainAsync_ExistingProjectId_ReturnsNullAndLogsWarning()
        {
            // Arrange
            using var context = new YsmDbContext(_dbContextOptions);
            var repository = new ImageRepository(context, _loggerMock.Object);
            var projectId = Guid.NewGuid();

            // Act
            var result = await repository.GetMainAsync(projectId);

            // Assert

            Assert.Null(result);
            _loggerMock.VerifyLog(logger => logger.LogWarning($"Project with ID {projectId} doesn't have a main image.", projectId));
        }

        [Fact]
        public async Task AddAsync_ImageEntity_AddsImageRecordToDatabaseAndLogsInformation()
        {
            // Arrange
            using var context = new YsmDbContext(_dbContextOptions);
            var repository = new ImageRepository(context, _loggerMock.Object);
            string imageId = "ImageId";
            var newImage = new ImageEntity
            {
                ImageId = imageId,
                ImageURL = "ImageURL",
                ProjectId = Guid.NewGuid()
            };

            // Act
            await repository.AddAsync(newImage);
            await context.SaveChangesAsync();

            // Assert
            var imageInDb = await context.Images.FirstOrDefaultAsync(i => i.ImageId == imageId);
            Assert.NotNull(imageInDb);
            Assert.Equal(imageId, imageInDb.ImageId);
            _loggerMock.VerifyLog(logger => logger.LogInformation($"New image with ID {imageInDb.ImageId} was added.", imageInDb));
        }

        [Fact]
        public async Task DeleteAsync_ExistingImageId_DeletesImageRecordFromDatabase()
        {
            // Arrange
            using var context = new YsmDbContext(_dbContextOptions);
            var repository = new ImageRepository(context, _loggerMock.Object);
            var testImage = new ImageEntity 
            { 
                ImageId = "ImageId1", 
                ImageURL = "ImageURL", 
                ProjectId = Guid.NewGuid() 
            };

            context.Images.Add(testImage);
            await context.SaveChangesAsync();

            // Act
            await repository.DeleteAsync("ImageId1");
            await context.SaveChangesAsync();

            // Assert
            var imageInDb = await context.Images.FirstOrDefaultAsync(i => i.ImageId == "ImageId1");
            Assert.Null(imageInDb);
            _loggerMock.VerifyLog(logger => logger.LogInformation($"Image record with ID {testImage.ImageId} was succesfully deleted.", testImage));
        }

        [Fact]
        public async Task DeleteAsync_NonExistingImageId_LogsWarning()
        {
            // Arrange
            using var context = new YsmDbContext(_dbContextOptions);
            var repository = new ImageRepository(context, _loggerMock.Object);

            // Act
            await repository.DeleteAsync("ImageId1");
            await context.SaveChangesAsync();

            // Assert
            _loggerMock.VerifyLog(logger => logger.LogWarning("Image with ID ImageId1 wasn't found."));
        }
    }
}
