using AutoMapper;
using Todo_App.Application.Common.Mappings;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.TodoLists.Queries.GetTodos;

public class TodoListDto : IMapFrom<TodoList>
{
    public TodoListDto()
    {
        Items = new List<TodoItemDto>();
    }

    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Colour { get; set; }

    public IList<TodoItemDto> Items { get; set; }
   
        public void Mapping(Profile profile)
        {
            profile.CreateMap<TodoList, TodoListDto>()
                .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items.Where(i => i.IsDeleted == false)));
        }
    
}
