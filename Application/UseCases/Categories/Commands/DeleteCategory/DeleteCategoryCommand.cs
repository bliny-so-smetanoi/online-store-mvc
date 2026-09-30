using Application.Abstractions;
using Application.Repositories.ICategoryRepository;
using CSharpFunctionalExtensions;

namespace Application.UseCases.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : ICommand<Result>;

public class DeleteCategoryCommandHandler(ICategoryRepository repository) : ICommandHandler<DeleteCategoryCommand, Result>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
            return Result.Failure("CategoryNotFound");
        if (await repository.IsInUseAsync(request.Id, cancellationToken))
            return Result.Failure("CategoryInUse");

        await repository.DeleteAsync(category, cancellationToken);
        return Result.Success();
    }
}