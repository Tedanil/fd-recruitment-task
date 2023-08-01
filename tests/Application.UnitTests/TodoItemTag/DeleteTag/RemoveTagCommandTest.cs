using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.TodoItemTag.Commands.DeleteTag;
using static Todo_App.Application.TodoItemTag.Commands.DeleteTag.RemoveTagCommand;

namespace Todo_App.Application.UnitTests.TodoItemTag.DeleteTag;
public class RemoveTagCommandTest
{
    [Test]
    public async Task ShouldRemoveTagAndSaveChanges()
    {
        //ARRANGE 
        var todoItemId = 1;
        var tagId = 1;
        var cancellationToken = CancellationToken.None;

        var mockDbContext = new Mock<IApplicationDbContext>();

        var testEntity = new Domain.Entities.TodoItemTag { TodoItemId = todoItemId, TagId = tagId };
        var data = new List<Domain.Entities.TodoItemTag> { testEntity };

        var queryableList = data.AsQueryable();
        var mockDbSet = new Mock<DbSet<Domain.Entities.TodoItemTag>>();
        mockDbSet.As<IQueryable<Domain.Entities.TodoItemTag>>().Setup(m => m.Provider).Returns(queryableList.Provider);
        mockDbSet.As<IQueryable<Domain.Entities.TodoItemTag>>().Setup(m => m.Expression).Returns(queryableList.Expression);
        mockDbSet.As<IQueryable<Domain.Entities.TodoItemTag>>().Setup(m => m.ElementType).Returns(queryableList.ElementType);
        mockDbSet.As<IQueryable<Domain.Entities.TodoItemTag>>().Setup(m => m.GetEnumerator()).Returns(queryableList.GetEnumerator());

        mockDbContext.Setup(m => m.TodoItemTags).Returns(mockDbSet.Object);
        var removeTagCommandHandler = new RemoveTagCommandHandler(mockDbContext.Object);

        //ACT
        await removeTagCommandHandler.Handle(new RemoveTagCommand(todoItemId, tagId), cancellationToken);

        //ASSERT
        mockDbContext.Verify(m => m.SaveChangesAsync(cancellationToken), Times.Once);
    }
}






