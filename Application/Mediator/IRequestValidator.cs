using CSharpFunctionalExtensions;

namespace Application.Mediator;

public interface IRequestValidator<in TRequest> {
    Task<Result> RequestValidateAsync(TRequest request, CancellationToken cancellationToken);
    Task<Result> RequestValidateFluentRulesAsync(TRequest request, CancellationToken cancellationToken);
}