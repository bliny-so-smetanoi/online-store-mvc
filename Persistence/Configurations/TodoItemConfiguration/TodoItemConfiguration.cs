using Domain.Entities.TodoItem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Configurations.Abstract;

namespace Persistence.Configurations.TodoItemConfiguration;

public class TodoItemConfiguration : AuditableEntityConfiguration<TodoItem>
{
    public override void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.ToTable("TodoItem");
        builder.HasKey(x => x.Id);
        
    }
}