using YSMConcept.Application.Interfaces;
using YSMConcept.Domain.RepositoryInterfaces;
using YSMConcept.Infrastructure.Data;

namespace YSMConcept.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly YsmDbContext _dbContext;
        public IProjectRepository Projects { get; }
        public IImageRepository Images { get; }

        public UnitOfWork(
            YsmDbContext dbContext, 
            IProjectRepository projects, 
            IImageRepository images)
        {
            _dbContext = dbContext;
            Projects = projects;
            Images = images;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync()
        {
            await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _dbContext.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _dbContext.Database.RollbackTransactionAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }

}
