using System.ComponentModel.DataAnnotations;

namespace Application.Services.IVisitorService.Dtos;

public sealed class VisitorCodeRequestVm
{
    [Required]
    public string Phone { get; set; } = default!;
}
