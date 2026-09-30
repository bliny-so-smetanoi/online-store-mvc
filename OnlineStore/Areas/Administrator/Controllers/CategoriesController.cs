using Application.UseCases.Categories.Commands.DeleteCategory;
using Application.UseCases.Categories.Commands.SaveCategory;
using Application.UseCases.Categories.Dtos;
using Application.UseCases.Categories.Queries.GetCategories;
using KeycloakExtension;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Resource;

namespace OnlineStore.Areas.Administrator.Controllers;

[Authorize(Roles = RoleConstants.Root)]
[Area("Administrator")]
[Route("[area]/[controller]")]
public class CategoriesController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCategoriesQuery(), cancellationToken);
        return View(result.Value);
    }

    [HttpGet("Add")]
    public async Task<IActionResult> Add(CancellationToken cancellationToken)
    {
        await PopulateParents(null, cancellationToken);
        return View("Edit", new CategoryDto());
    }

    [HttpGet("Edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var categories = await mediator.Send(new GetCategoriesQuery(), cancellationToken);
        var category = categories.Value.SingleOrDefault(x => x.Id == id);
        if (category is null)
            return NotFound();
        await PopulateParents(id, cancellationToken);
        return View(category);
    }

    [HttpPost("Add")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Create(CategoryDto data, CancellationToken cancellationToken) =>
        Save(null, data, cancellationToken);

    [HttpPost("Edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Update(Guid id, CategoryDto data, CancellationToken cancellationToken) =>
        Save(id, data, cancellationToken);

    [HttpGet("Delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var categories = await mediator.Send(new GetCategoriesQuery(), cancellationToken);
        var category = categories.Value.SingleOrDefault(x => x.Id == id);
        return category is null ? NotFound() : View(category);
    }

    [HttpPost("Delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteCategoryCommand(id), cancellationToken);
        if (result.IsSuccess)
            return RedirectToAction(nameof(Index));
        if (result.Error == "CategoryNotFound")
            return NotFound();

        ModelState.AddModelError(string.Empty, CatalogResources.Get(result.Error));
        return await Delete(id, cancellationToken);
    }

    private async Task<IActionResult> Save(Guid? id, CategoryDto data, CancellationToken cancellationToken)
    {
        data.Id = id ?? Guid.Empty;
        // An empty description is allowed; normalize the MVC empty-string binding.
        data.Description ??= string.Empty;
        ModelState.Remove(nameof(data.Description));
        if (ModelState.IsValid)
        {
            var result = await mediator.Send(new SaveCategoryCommand(id, data), cancellationToken);
            if (result.IsSuccess)
                return RedirectToAction(nameof(Index));
            if (result.Error == "CategoryNotFound" && id.HasValue)
            {
                var categories = await mediator.Send(new GetCategoriesQuery(), cancellationToken);
                if (categories.Value.All(x => x.Id != id.Value))
                    return NotFound();
            }
            ModelState.AddModelError(string.Empty, CatalogResources.Get(result.Error));
        }
        await PopulateParents(id, cancellationToken);
        return View("Edit", data);
    }

    private async Task PopulateParents(Guid? id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCategoriesQuery(), cancellationToken);
        var excluded = new HashSet<Guid>();
        if (id.HasValue)
        {
            excluded.Add(id.Value);
            bool changed;
            do
            {
                changed = false;
                foreach (var category in result.Value)
                    if (category.ParentId.HasValue && excluded.Contains(category.ParentId.Value))
                        changed |= excluded.Add(category.Id);
            } while (changed);
        }
        ViewBag.Parents = new SelectList(result.Value.Where(x => !excluded.Contains(x.Id)), "Id", "Name");
    }
}