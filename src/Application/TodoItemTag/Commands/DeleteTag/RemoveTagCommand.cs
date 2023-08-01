using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;

namespace Todo_App.Application.TodoItemTag.Commands.DeleteTag;
public class RemoveTagCommand : IRequest
{
    public int TodoItemId { get; set; }
    public int TagId { get; set; }

    public RemoveTagCommand(int todoItemId, int tagId)
    {
        TodoItemId = todoItemId;
        TagId = tagId;
    }

    public class RemoveTagCommandHandler : IRequestHandler<RemoveTagCommand>
    {
        private readonly IApplicationDbContext _context;

        public RemoveTagCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(RemoveTagCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.TodoItemTags
                .Where(t => t.TodoItemId == request.TodoItemId && t.TagId == request.TagId)
                .SingleOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(TodoItemTag), request.TagId);
            }

            _context.TodoItemTags.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }

}
