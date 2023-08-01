using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo_App.Application.TodoLists.Queries.GetTodos;

namespace Todo_App.Application.TodoItems.Queries.GetTodoItemsByTagInList;
public class TodosByTagInListVm
{
    public IList<TodoItemDto> Items { get; set; } = new List<TodoItemDto>();
}

