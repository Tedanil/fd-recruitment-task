using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.TodoItemTag.Commands.CreateTag;
public class CreateTagCommand : IRequest<int>
{
    public int TodoItemId { get; set; }
    public string TagName { get; set; }
}

public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateTagCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var todoItem = await _context.TodoItems.FindAsync(request.TodoItemId);
        if (todoItem == null)
        {
            throw new NotFoundException(nameof(TodoItem), request.TodoItemId);
        }

        var existingTag = await _context.Tags
            .FirstOrDefaultAsync(t => t.Name == request.TagName, cancellationToken);

        Tag tag;

        if (existingTag != null)
        {
            
            tag = existingTag;
        }
        else
        {
            
            tag = new Tag { Name = request.TagName };
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync(cancellationToken);
        }

        
        var existingTodoItemTag = await _context.TodoItemTags
            .FirstOrDefaultAsync(t => t.TodoItemId == todoItem.Id && t.TagId == tag.Id, cancellationToken);

        if (existingTodoItemTag != null)
        {
            
            throw new InvalidOperationException("This tag is already assigned to the todo item.");
        }

        var todoItemTag = new Domain.Entities.TodoItemTag { TodoItemId = todoItem.Id, TagId = tag.Id };

        _context.TodoItemTags.Add(todoItemTag);

        await _context.SaveChangesAsync(cancellationToken);

        return tag.Id;
    }



}

