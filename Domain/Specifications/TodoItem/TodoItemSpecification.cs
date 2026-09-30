using NSpecifications;

namespace Domain.Specifications.TodoItem;

public static class TodoItemSpecification
{
    
    public static Spec<Entities.TodoItem.TodoItem> GetById(Guid id)
    {
        return new Spec<Entities.TodoItem.TodoItem>(x => x.Id == id);
    }
}