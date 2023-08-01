using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Todo_App.Application.Tags.Queries.GetAllTags;
using Todo_App.Application.Tags.Queries.GetTopTags;

namespace Todo_App.WebUI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TagsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<TopTagsVm>> GetTopTags()
    {
        return await Mediator.Send(new GetTopTagsQuery());
    }

    [HttpGet("AllTags")]
    public async Task<ActionResult<AllTagsVm>> GetAllTags()
    {
        return await Mediator.Send(new GetAllTagsQuery());
    }

}
