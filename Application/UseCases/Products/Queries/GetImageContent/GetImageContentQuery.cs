using Application.Abstractions;
using Application.Repositories.IProductRepository;
using CSharpFunctionalExtensions;

namespace Application.UseCases.Products.Queries.GetImageContent;

public record GetImageContentQuery(Guid ImageId) : IQuery<Result<byte[]>>;

public class GetImageContentQueryHandler(IProductRepository productRepository) : IQueryHandler<GetImageContentQuery, Result<byte[]>>
{
    public async Task<Result<byte[]>> Handle(GetImageContentQuery request, CancellationToken cancellationToken)
    {
        var image = await productRepository.GetImageAsync(request.ImageId, cancellationToken);

        if (image is null)
        {
            return Result.Failure<byte[]>("Image not found");
        }
        
        return image.Content;
    }
}