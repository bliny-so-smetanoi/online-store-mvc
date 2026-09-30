using System.ComponentModel.DataAnnotations;

namespace Application.Services.IVisitorService.Dtos;

public sealed class VisitorLoginVm
{
    [Required]
    public string Phone { get; set; } = default!;

    [Required]
    public string Code { get; set; } = default!;

    // public string ReturnUrl { get; set; } = "/";
}