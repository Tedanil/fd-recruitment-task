using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo_App.Domain.Entities;

namespace Todo_App.Infrastructure.Persistence.Configurations;
public class TodoItemTagConfiguration : IEntityTypeConfiguration<TodoItemTag>
{
    public void Configure(EntityTypeBuilder<TodoItemTag> builder)
    {
        builder.HasKey(tt => new { tt.TodoItemId, tt.TagId });

        builder
            .HasOne(tt => tt.TodoItem)
            .WithMany(ti => ti.TodoItemTags)
            .HasForeignKey(tt => tt.TodoItemId);

        builder
            .HasOne(tt => tt.Tag)
            .WithMany(t => t.TodoItemTags)
            .HasForeignKey(tt => tt.TagId);
    }
}
