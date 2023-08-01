using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.TodoLists.Queries.GetTodos;

namespace Todo_App.Application.TodoItems.Queries.GetTodoItemsByTagInList;
public record GetTodoItemsByTagInListQuery(int TagId, int ListId) : IRequest<TodosByTagInListVm>;

public class GetTodoItemsByTagInListQueryHandler : IRequestHandler<GetTodoItemsByTagInListQuery, TodosByTagInListVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTodoItemsByTagInListQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TodosByTagInListVm> Handle(GetTodoItemsByTagInListQuery request, CancellationToken cancellationToken)
    {
        // Get the todo items that have the specified tag and are in the specified list
        var todoItemsWithGivenTagInList = await _context.TodoItemTags
            .Include(tit => tit.TodoItem)
            .Where(tit => tit.TagId == request.TagId && tit.TodoItem.ListId == request.ListId && tit.TodoItem.IsDeleted == false)
            .Select(tit => tit.TodoItem)
            .ToListAsync(cancellationToken);

        // Map the todo items to their DTOs
        var mappedTodoItems = _mapper.Map<List<TodoItemDto>>(todoItemsWithGivenTagInList);

        return new TodosByTagInListVm
        {
            Items = mappedTodoItems
        };
    }
}


