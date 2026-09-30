using Application.Abstractions;
using Application.UseCases.Products.Dtos;
using CSharpFunctionalExtensions;

namespace Application.UseCases.Products.Commands.UpdateProduct;

public record UpdateProductCommand(Guid Id, UpdateProductDto Data) : ICommand<Result>;