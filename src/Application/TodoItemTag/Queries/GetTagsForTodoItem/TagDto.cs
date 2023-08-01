using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Todo_App.Application.Common.Mappings;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.TodoItemTag.Queries.GetTagsForTodoItem;
public class TagDto : IMapFrom<Tag>
{
    public int Id { get; set; }
    public string Name { get; set; }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Tag, TagDto>();
        }
    }

}