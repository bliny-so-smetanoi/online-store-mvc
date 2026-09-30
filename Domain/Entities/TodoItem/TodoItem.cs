using Domain.SeedWork;

namespace Domain.Entities.TodoItem;

public class TodoItem : AuditableEntity
{
    public string Title { get; set; }

    
}