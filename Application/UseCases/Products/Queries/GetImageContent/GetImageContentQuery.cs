using Application.Abstractions;
using Application.Repositories.IProductRepository;
using Application.UseCases.Products.Dtos;
using CSharpFunctionalExtensions;

namespace Application.UseCases.Products.Queries.GetImageContent;

public record GetImageContentQuery(Guid ImageId) : IQuery<Result<ImageDto>>;

public class GetImageContentQueryHandler(IProductRepository productRepository) : IQueryHandler<GetImageContentQuery, Result<ImageDto>>
{
    public async Task<Result<ImageDto>> Handle(GetImageContentQuery request, CancellationToken cancellationToken)
    {
        var image = await productRepository.GetImageAsync(request.ImageId, cancellationToken);

        if (image is null)
        {
            return Result.Failure<ImageDto>("Image not found");
        }
        
        return new ImageDto { Content = image.Content, FileName = image.FileName, Size = image.Size };
    }
}
