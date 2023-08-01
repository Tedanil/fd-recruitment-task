using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.TodoLists.Commands.DeleteTodoList;

namespace Todo_App.Application.UnitTests.TodoList.DeleteTodoList;
public class DeleteTodoListCommandTest
{
    [Test]
    public async Task ShouldDeleteListAndSaveChanges()
    {
        // ARRANGE 
        var listId = 1;
        var cancellationToken = CancellationToken.None;

        var testEntity = new Domain.Entities.TodoList { Id = listId };
        var testList = new List<Domain.Entities.TodoList>() { testEntity };

        var mockDbSet = new Mock<DbSet<Domain.Entities.TodoList>>();
        mockDbSet.As<IQueryable<Domain.Entities.TodoList>>().Setup(m => m.Provider).Returns(testList.AsQueryable().Provider);
        mockDbSet.As<IQueryable<Domain.Entities.TodoList>>().Setup(m => m.Expression).Returns(testList.AsQueryable().Expression);
        mockDbSet.As<IQueryable<Domain.Entities.TodoList>>().Setup(m => m.ElementType).Returns(testList.AsQueryable().ElementType);
        mockDbSet.As<IQueryable<Domain.Entities.TodoList>>().Setup(m => m.GetEnumerator()).Returns(testList.AsQueryable().GetEnumerator());
        mockDbSet.Setup(m => m.FindAsync(new object[] { listId }, cancellationToken)).ReturnsAsync(testEntity);

        var mockDbContext = new Mock<IApplicationDbContext>();
        mockDbContext.Setup(m => m.TodoLists).Returns(mockDbSet.Object);

        var deleteCommandHandler = new DeleteTodoListCommandHandler(mockDbContext.Object);

        // ACT
        await deleteCommandHandler.Handle(new DeleteTodoListCommand(listId), cancellationToken);

        // ASSERT
        Assert.IsTrue(testEntity.IsDeleted);
        mockDbContext.Verify(m => m.SaveChangesAsync(cancellationToken), Times.Once);
    }
}


