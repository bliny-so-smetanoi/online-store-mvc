using System.Globalization;
using Application.Repositories;
using Application.Repositories.ITodoItemRepository;
using Application.Repositories.ITodoItemRepository.Dtos;
using Application.UseCases.TodoItem.Commands.Create;
using Application.UseCases.TodoItem.Dtos;
using Application.UseCases.TodoItem.Queries.GetAll;
using Application.UseCases.Products.Dtos;
using Application.UseCases.Products.Queries.GetProductsVisitor;
using Application.UseCases.Products.Queries.GetImageContent;
using KeycloakExtension;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Localization;
using OnlineStore.Helpers;
using Resource;

namespace OnlineStore.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IMediator _mediator;
    private readonly ITodoItemRepository _repository;
    private readonly IStringLocalizer<SharedResources> _localizer;
    
    public HomeController(ILogger<HomeController> logger, 
                            IMediator mediator, 
                            ITodoItemRepository todoItemRepository,
                            IStringLocalizer<SharedResources> localizer
                            )
    {
        _logger = logger;
        _mediator = mediator;
        _repository = todoItemRepository;
        _localizer = localizer; 
    }
    
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(CancellationToken cancellationToken, int page = 1, string? searchString = null)
    {
        if (!ModelState.IsValid || page < 1 || page > int.MaxValue / 6)
            return BadRequest();

        var filters = new GetAllProductsFilterDto { Page = page, PageSize = 6 };
        var products = await _mediator.Send(new GetProductsVisitorQuery(filters, searchString), cancellationToken);
        return Ok(products);
    }

    [HttpGet]
    public async Task<IActionResult> ProductImage(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetImageContentQuery(id), cancellationToken);
        if (result.IsFailure)
            return NotFound();

        var image = result.Value;
        if (!new FileExtensionContentTypeProvider().TryGetContentType(image.FileName, out var contentType)
            || !contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return NotFound();

        return File(image.Content, contentType);
    }
    
    public IActionResult About()
    {
        var test = User.Identities;
        return View();
    }
    
    public IActionResult Forbidden()
    {
        return View();
    }
}
