using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.TodoItemTag.Commands.CreateTag;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.UnitTests.TodoItemTag.CreateTag;
public class CreateTagCommandHandlerTest
{
    [Test]
    public async Task ShouldCreateTagAndSaveChanges()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var mockDbContext = new Mock<IApplicationDbContext>();
        var mockTodoItemsDbSet = new Mock<DbSet<Domain.Entities.TodoItem>>();
        var mockTagsDbSet = new Mock<DbSet<Tag>>();
        var mockTodoItemTagsDbSet = new Mock<DbSet<Domain.Entities.TodoItemTag>>();

        var todoItem = new Domain.Entities.TodoItem { Id = 1 };
        var tagName = "New Tag";

        var command = new CreateTagCommand { TodoItemId = todoItem.Id, TagName = tagName };

        mockTodoItemsDbSet.Setup(dbSet => dbSet.FindAsync(new object[] { todoItem.Id }, cancellationToken)).ReturnsAsync(todoItem);
        mockDbContext.Setup(db => db.TodoItems).Returns(mockTodoItemsDbSet.Object);
        mockDbContext.Setup(db => db.Tags).Returns(mockTagsDbSet.Object);
        mockDbContext.Setup(db => db.TodoItemTags).Returns(mockTodoItemTagsDbSet.Object);

        var handler = new CreateTagCommandHandler(mockDbContext.Object);

        // Act
        var tagId = await handler.Handle(command, cancellationToken);

        // Assert
        mockTagsDbSet.Verify(dbSet => dbSet.Add(It.IsAny<Tag>()), Times.Once);
        mockTodoItemTagsDbSet.Verify(dbSet => dbSet.Add(It.IsAny<Domain.Entities.TodoItemTag>()), Times.Once);
        mockDbContext.Verify(db => db.SaveChangesAsync(cancellationToken), Times.Exactly(2));
    }

}


