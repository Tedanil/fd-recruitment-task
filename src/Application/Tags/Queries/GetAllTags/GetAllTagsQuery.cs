using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.TodoItemTag.Queries.GetTagsForTodoItem;

namespace Todo_App.Application.Tags.Queries.GetAllTags;
public record GetAllTagsQuery : IRequest<AllTagsVm>;

public class GetAllTagsQueryHandler : IRequestHandler<GetAllTagsQuery, AllTagsVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllTagsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<AllTagsVm> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _context.Tags
            .Select(t => new TagDto { Id = t.Id, Name = t.Name })
            .ToListAsync(cancellationToken);

        return new AllTagsVm { Tags = tags };
    }
}



