using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo_App.Application.TodoItemTag.Queries.GetTagsForTodoItem;

namespace Todo_App.Application.Tags.Queries.GetAllTags;
public class AllTagsVm
{
    public IList<TagDto> Tags { get; set; } = new List<TagDto>();
}
