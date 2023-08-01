using Microsoft.EntityFrameworkCore;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }
    DbSet<Tag> Tags { get; } // Yeni eklenen Tag DbSet
    DbSet<Domain.Entities.TodoItemTag> TodoItemTags { get; } // Yeni eklenen TodoItemTag DbSet


    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
