using Application.UseCases.Products.Commands.AddProduct;
using Application.UseCases.Products.Commands.UpdateProduct;
using Application.UseCases.Products.Dtos;
using Application.UseCases.Products.Queries.GetProductById;
using Application.UseCases.Products.Queries.GetProducts;
using KeycloakExtension;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Helpers;

namespace OnlineStore.Areas.Administrator.Controllers;

[Authorize(Roles = RoleConstants.Root)]
[Area("Administrator")]
[Route("[area]/[controller]")]
public class ProductsController(IMediator mediator) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("Add")]
    public IActionResult Add()
    {
        return View();
    }

    [HttpGet("Edit/{id:guid}")]
    public IActionResult Edit(Guid id)
    {
        return View();
    }

    [HttpPost("AddProduct")]
    public async Task<IActionResult> AddProduct([FromForm] AddProductFormDto form, CancellationToken cancellationToken)
    {
        var dto = await form.ToProductDtoAsync();
        var command = new AddProductCommand(dto);
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Created();
    }

    [HttpGet("GetList")]
    public async Task<IActionResult> GetList(GetAllProductsFilterDto filter, CancellationToken cancellationToken)
    {
        var query = new GetProductsQuery(filter);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("GetForUpdate/{id:guid}")]
    public async Task<IActionResult> GetForUpdate(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProductForUpdateByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return result.AsHttpResult();
    }

    [HttpPut("Update/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdateProductFormDto form, CancellationToken cancellationToken)
    {
        var dto = await form.ToProductDtoAsync();
        var command = new UpdateProductCommand(id, dto);
        var result = await mediator.Send(command, cancellationToken);
        return result.AsHttpResult();
    }
}

public class UpdateProductFormDto : AddProductFormDto
{
    public new async Task<UpdateProductDto> ToProductDtoAsync()
    {
        var imageDtos = new List<ImageDto>();
        
        foreach (var image in Images)
        {
            await using var memoryStream = new MemoryStream();
            memoryStream.Position = 0;
            await image.CopyToAsync(memoryStream);
            var imageDto = new ImageDto
            {
                Content = memoryStream.ToArray(),
                FileName = image.FileName,
                Size = memoryStream.Length
            };
            imageDtos.Add(imageDto);
        }
        
        return new UpdateProductDto
        {
            Name = this.Name,
            Price = this.Price,
            Description = this.Description,
            Quantity = this.Quantity,
            Images = imageDtos
        };
    }
}

public class AddProductFormDto
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public List<IFormFile> Images { get; set; } = new List<IFormFile>();

    public async Task<AddProductDto> ToProductDtoAsync()
    {
        var imageDtos = new List<ImageDto>();
        
        foreach (var image in Images)
        {
            await using var memoryStream = new MemoryStream();
            memoryStream.Position = 0;
            await image.CopyToAsync(memoryStream);
            var imageDto = new ImageDto
            {
                Content = memoryStream.ToArray(),
                FileName = image.FileName,
                Size = memoryStream.Length
            };
            imageDtos.Add(imageDto);
        }
        
        return new AddProductDto
        {
            Name = this.Name,
            Price = this.Price,
            Description = this.Description,
            Quantity = this.Quantity,
            Images = imageDtos
        };

    }
}