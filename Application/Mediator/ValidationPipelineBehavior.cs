using CSharpFunctionalExtensions;
using MediatR;

namespace Application.Mediator;

public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse> where TResponse : IResult {
    private readonly IEnumerable<IRequestValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IRequestValidator<TRequest>> validators) {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
        foreach (var validator in _validators) {
            var validateFluentRulesResult = await validator.RequestValidateFluentRulesAsync(request, cancellationToken);
            if (validateFluentRulesResult.IsFailure) {
                throw new ApplicationException(validateFluentRulesResult.Error);
            }

            var requestValidateResult = await validator.RequestValidateAsync(request, cancellationToken);
            if (requestValidateResult.IsFailure) {
                throw new ApplicationException(validateFluentRulesResult.Error);
            }
        }

        return await next();
    }
}