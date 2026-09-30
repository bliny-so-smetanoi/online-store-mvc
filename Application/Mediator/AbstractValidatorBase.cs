using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Mediator;

public abstract class RequestValidatorBase<T> : AbstractValidator<T>, IRequestValidator<T> {
    public sealed override ValidationResult Validate(ValidationContext<T> context) {
        return base.Validate(context);
    }

    public sealed override Task<ValidationResult> ValidateAsync(ValidationContext<T> context, CancellationToken cancellation = new CancellationToken()) {
        return base.ValidateAsync(context, cancellation);
    }

    public virtual Task<Result> RequestValidateAsync(T request, CancellationToken cancellationToken) {
        return Task.FromResult(Result.Success());
    }

    public async Task<Result> RequestValidateFluentRulesAsync(T request, CancellationToken cancellationToken) {
        var fluentValidationResult = await base.ValidateAsync(request, cancellationToken);
        if (!fluentValidationResult.IsValid) {
            var errorText = string.Join(Environment.NewLine, fluentValidationResult.Errors.Select(x => $"{x.ErrorMessage}"));
            return Result.Failure(errorText);
        }

        return Result.Success();
    }
}