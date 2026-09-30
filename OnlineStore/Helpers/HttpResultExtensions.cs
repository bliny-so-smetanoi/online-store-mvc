using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;

namespace OnlineStore.Helpers;

public static class HttpResultExtensions {
    public static IActionResult AsHttpResult(this Result result) {
        if (result.IsFailure) {
            return new BadRequestObjectResult(result.Error);
        }
        return new OkResult();
    }

    public static IActionResult AsHttpResult<T>(this Result<T> result) {
        if (result.IsFailure) {
            return new BadRequestObjectResult(result.Error);
        }
        return new OkObjectResult(result.Value);
    }
}