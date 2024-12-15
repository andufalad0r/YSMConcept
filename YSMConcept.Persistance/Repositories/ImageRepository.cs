using YSMConcept.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using YSMConcept.Infrastructure.Data;
using YSMConcept.Domain.RepositoryInterfaces;

namespace YSMConcept.Infrastructure.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly YsmDbContext _dbContext;
        private readonly ILogger<ImageRepository> _logger;

        public ImageRepository(
            YsmDbContext dbContext, 
            ILogger<ImageRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ImageEntity?> GetByIdAsync(string imageId)
        {
            var imageEntity = await _dbContext.Images
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ImageId == imageId);

            if (imageEntity == null)
                _logger.LogWarning($"Image with ID {imageId} not found.", imageId);

            return imageEntity;
        }

        public async Task<List<ImageEntity>> GetAllAsync(int pageNumber, int pageSize)
        {
            int skipPages = pageNumber * pageSize;

            var imagesEntities = await _dbContext.Images
                .AsNoTracking()
                .Skip(skipPages)
                .Take(pageSize)
                .ToListAsync();

            if (imagesEntities.Count == 0)
                _logger.LogWarning("No images found in the database.");

            return imagesEntities;
        }

        public async Task<List<ImageEntity>> GetAllByProjectIdAsync(Guid projectId)
        {
            var imagesEntities = await _dbContext.Images
                .AsNoTracking()
                .Where(p => p.ProjectId == projectId)
                .ToListAsync();

            if (imagesEntities.Count == 0)
                _logger.LogWarning($"No images found for project with Id {projectId} in the database.", projectId);

            return imagesEntities;
        }

        public async Task<ImageEntity?> GetMainAsync(Guid projectId)
        {
            var mainImageEntity = await _dbContext.Images.FirstOrDefaultAsync(image => image.ProjectId == projectId && image.IsMain == true);

            if (mainImageEntity == null)
                _logger.LogWarning($"Project with ID {projectId} doesn't have a main image.", mainImageEntity);

            return mainImageEntity;
        }

        public async Task AddAsync(ImageEntity imageEntity)
        {
            await _dbContext.AddAsync(imageEntity);
            _logger.LogInformation($"New image with ID {imageEntity.ImageId} was added.", imageEntity);
        }

        public async Task DeleteAsync(string imageId)
        {
            var imageEntity = await _dbContext.Images
                .FirstOrDefaultAsync(p => p.ImageId == imageId);
            if (imageEntity == null)
            {
                _logger.LogWarning($"Image with ID {imageId} wasn't found.", imageId);
                return;
            }
            _dbContext.Images.Remove(imageEntity);
            _logger.LogInformation($"Image record with ID {imageEntity.ImageId} was succesfully deleted.", imageEntity);
        }
    }
}
