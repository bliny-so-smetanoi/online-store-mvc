using System.Globalization;
using Application.Repositories;
using Application.Repositories.ITodoItemRepository;
using Application.Repositories.ITodoItemRepository.Dtos;
using Application.UseCases.TodoItem.Commands.Create;
using Application.UseCases.TodoItem.Dtos;
using Application.UseCases.TodoItem.Queries.GetAll;
using KeycloakExtension;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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