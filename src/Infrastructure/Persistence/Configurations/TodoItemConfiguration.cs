using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo_App.Domain.Entities;

namespace Todo_App.Infrastructure.Persistence.Configurations;

public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Note)
            .HasMaxLength(2);

        builder
            .OwnsOne(b => b.Colour);

        builder
               .HasMany(t => t.TodoItemTags)
               .WithOne(tt => tt.TodoItem)
               .HasForeignKey(tt => tt.TodoItemId);
    }
}
