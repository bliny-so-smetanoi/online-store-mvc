using Application.Abstractions;
using Application.UseCases.Products.Dtos;
using CSharpFunctionalExtensions;

namespace Application.UseCases.Products.Queries.GetProductById;

public record GetProductForUpdateByIdQuery(Guid Id) : IQuery<Result<UpdateViewProductDto>>;