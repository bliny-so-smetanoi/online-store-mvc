using Application.Abstractions;
using Application.UseCases.Products.Dtos;
using CSharpFunctionalExtensions;

namespace Application.UseCases.Products.Commands.AddProduct;

public record AddProductCommand(AddProductDto Data) : ICommand<Result>;