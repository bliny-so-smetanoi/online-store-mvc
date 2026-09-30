using Domain.Entities.Products;

namespace Application.Repositories.ICategoryRepository;

public interface ICategoryRepository
{
    Task CreateAsync(Category category, CancellationToken cancellationToken);
    Task<List<Category>> GetAllCategories(CancellationToken cancellationToken);
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(Category category, CancellationToken cancellationToken);
    Task<bool> IsInUseAsync(Guid id, CancellationToken cancellationToken);
    Task DeleteAsync(Category category, CancellationToken cancellationToken);
}