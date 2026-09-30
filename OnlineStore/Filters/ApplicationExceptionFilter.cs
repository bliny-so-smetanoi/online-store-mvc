using System.Net;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OnlineStore.Filters;

public class ApplicationExceptionFilter(ILogger<ApplicationExceptionFilter> logger) : ExceptionFilterAttribute
{
    public override Task OnExceptionAsync(ExceptionContext context) {
        switch (context.Exception) {
            case ApplicationException ex:
                logger.LogError(ex, "Exception: " + ex.Message);
                context.Result = new BadRequestObjectResult(ex.Message) { StatusCode = StatusCodes.Status400BadRequest };        
                break;
            default:
                var errorResult = new ObjectResult("Сервис недоступен, обратитесь в тех. поддержку.") {
                    StatusCode = (int) HttpStatusCode.InternalServerError
                };
                logger.LogError(context.Exception, "Unhandled exception. " + context.Exception.Message);
                context.Result = errorResult;
                break;
        }
        
        return base.OnExceptionAsync(context);
    }
}