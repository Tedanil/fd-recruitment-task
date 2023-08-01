using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Todo_App.Application.TodoItemTag.Commands.CreateTag;
using Todo_App.Application.TodoItemTag.Commands.DeleteTag;
using Todo_App.Application.TodoItemTag.Queries.GetTagsForTodoItem;

namespace Todo_App.WebUI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TodoItemTagController : ApiControllerBase
{
    [HttpGet("{todoItemId}")]
    public async Task<ActionResult<List<TagDto>>> GetTagsForTodoItem(int todoItemId)
    {
        var vm = await Mediator.Send(new GetTagsForTodoItemQuery(todoItemId));

        if (vm == null)
        {
            return NotFound();
        }

        return vm.Tags;
    }


    [HttpPost("{todoItemId}")]
    public async Task<ActionResult<int>> AddTagToTodoItem(int todoItemId, CreateTagCommand command)
    {
        command.TodoItemId = todoItemId;
        return await Mediator.Send(command);
    }

    [HttpDelete("{todoItemId}/{tagId}")]
    public async Task<IActionResult> RemoveTagFromTodoItem(int todoItemId, int tagId)
    {
        await Mediator.Send(new RemoveTagCommand(todoItemId, tagId));
        return NoContent();
    }


}
