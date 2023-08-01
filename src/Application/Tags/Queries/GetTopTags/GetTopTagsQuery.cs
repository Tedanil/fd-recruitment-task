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

namespace Todo_App.Application.Tags.Queries.GetTopTags;
public record GetTopTagsQuery : IRequest<TopTagsVm>;

public class GetTopTagsQueryHandler : IRequestHandler<GetTopTagsQuery, TopTagsVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTopTagsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TopTagsVm> Handle(GetTopTagsQuery request, CancellationToken cancellationToken)
    {
        var topTags = await _context.TodoItemTags
            .AsNoTracking()
            .GroupBy(t => t.Tag.Name)
            .Select(g => new { TagName = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(3)
            .ToListAsync(cancellationToken);

        var topTagNames = topTags.Select(t => t.TagName).ToList();

        var tags = await _context.Tags
            .Where(t => topTagNames.Contains(t.Name))
            .Select(t => new TagDto { Id = t.Id, Name = t.Name })
            .ToListAsync(cancellationToken);

        return new TopTagsVm { Tags = tags };
    }

}

