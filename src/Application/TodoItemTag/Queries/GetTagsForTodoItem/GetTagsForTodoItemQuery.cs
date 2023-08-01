using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Interfaces;

namespace Todo_App.Application.TodoItemTag.Queries.GetTagsForTodoItem;
public record GetTagsForTodoItemQuery(int TodoItemId) : IRequest<TodoItemTagsVm>;

public class GetTodoItemTagsQueryHandler : IRequestHandler<GetTagsForTodoItemQuery, TodoItemTagsVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTodoItemTagsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TodoItemTagsVm> Handle(GetTagsForTodoItemQuery request, CancellationToken cancellationToken)
    {
        var todoItemTags = await _context.TodoItems
            .Where(t => t.Id == request.TodoItemId)
            .Include(t => t.TodoItemTags)
            .ThenInclude(t => t.Tag)
            .Select(t => t.TodoItemTags.Select(tag => tag.Tag))
            .FirstOrDefaultAsync(cancellationToken);

        if (todoItemTags is null)
        {
            // TodoItem bulunamadı veya TodoItem'in hiç tag'ı yok.
            return null;
        }




        var tagDtoList = _mapper.Map<List<TagDto>>(todoItemTags);
        var vm = new TodoItemTagsVm { Tags = tagDtoList };

        return vm;
    }
}
