using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.TodoItems.Commands.DeleteTodoItem;
using Todo_App.Infrastructure.Persistence;

namespace Todo_App.Application.UnitTests.TodoItem.Delete;
public class DeleteTodoItemCommandTest
{
    [Test]
    public async Task ShouldDeleteItemAndSaveChanges()
    {
        //ARRANGE 
        var itemId = 1;
        var cancellationToken = CancellationToken.None;
        var mockDbContext = new Mock<IApplicationDbContext>();
        var mockDbSet = new Mock<DbSet<Domain.Entities.TodoItem>>();

        mockDbContext.Setup(m => m.TodoItems).Returns(mockDbSet.Object);
        var deletecommandhandler = new DeleteTodoItemCommandHandler(mockDbContext.Object);

        var testEntity = new Domain.Entities.TodoItem { Id = itemId };
        mockDbSet.Setup(m => m.FindAsync(new object[] { itemId }, cancellationToken)).ReturnsAsync(testEntity);

        //ACT
        await deletecommandhandler.Handle(new DeleteTodoItemCommand (itemId) , cancellationToken);

        //ASSERT
        Assert.IsTrue(testEntity.IsDeleted);
        mockDbContext.Verify(m=> m.SaveChangesAsync(cancellationToken),Times.Once);
    }
}
