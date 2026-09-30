using Application.Abstractions;
using Application.Repositories.ICategoryRepository;
using Application.UseCases.Categories.Dtos;
using CSharpFunctionalExtensions;
using Domain.Entities.Products;
using FluentValidation;

namespace Application.UseCases.Categories.Commands.SaveCategory;

public record SaveCategoryCommand(Guid? Id, CategoryDto Data) : ICommand<Result>;

public class CategoryValidator : AbstractValidator<CategoryDto>
{
    public CategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("CategoryNameRequired")
            .MaximumLength(200).WithMessage("CategoryNameTooLong");
        RuleFor(x => x.Description).MaximumLength(2000).WithMessage("CategoryDescriptionTooLong");
    }
}

public class SaveCategoryCommandHandler(ICategoryRepository repository) : ICommandHandler<SaveCategoryCommand, Result>
{
    public async Task<Result> Handle(SaveCategoryCommand request, CancellationToken cancellationToken)
    {
        var validation = await new CategoryValidator().ValidateAsync(request.Data, cancellationToken);
        if (!validation.IsValid)
            return Result.Failure(validation.Errors[0].ErrorMessage);

        Category? category = null;
        if (request.Id.HasValue)
        {
            category = await repository.GetByIdAsync(request.Id.Value, cancellationToken);
            if (category is null)
                return Result.Failure("CategoryNotFound");
        }

        Category? parent = null;
        if (request.Data.ParentId.HasValue)
        {
            parent = await repository.GetByIdAsync(request.Data.ParentId.Value, cancellationToken);
            if (parent is null)
                return Result.Failure("CategoryNotFound");

            var categories = await repository.GetAllCategories(cancellationToken);
            var parents = categories.ToDictionary(x => x.Id, x => x.Parent?.Id);
            var visited = new HashSet<Guid>();
            Guid? current = parent.Id;
            while (current.HasValue)
            {
                if (current == request.Id || !visited.Add(current.Value))
                    return Result.Failure("CategoryCycle");
                current = parents.GetValueOrDefault(current.Value);
            }
        }

        var name = request.Data.Name.Trim();
        var description = request.Data.Description?.Trim() ?? string.Empty;
        if (category is null)
        {
            await repository.CreateAsync(Category.Create(name, description, parent), cancellationToken);
        }
        else
        {
            category.Update(name, description, parent);
            await repository.UpdateAsync(category, cancellationToken);
        }
        return Result.Success();
    }
}