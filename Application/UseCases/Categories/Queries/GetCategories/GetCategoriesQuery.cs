using Application.Abstractions;
using Application.Repositories.ICategoryRepository;
using Application.UseCases.Categories.Dtos;
using CSharpFunctionalExtensions;

namespace Application.UseCases.Categories.Queries.GetCategories;

public record GetCategoriesQuery : IQuery<Result<List<CategoryDto>>>;

public class GetCategoriesQueryHandler(ICategoryRepository repository)
    : IQueryHandler<GetCategoriesQuery, Result<List<CategoryDto>>>
{
    public async Task<Result<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await repository.GetAllCategories(cancellationToken);
        return categories.Select(x => new CategoryDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            ParentId = x.Parent?.Id,
            ParentName = x.Parent?.Name
        }).ToList();
    }
}