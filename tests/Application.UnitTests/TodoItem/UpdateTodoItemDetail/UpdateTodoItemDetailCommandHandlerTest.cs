using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.TodoItems.Commands.UpdateTodoItemDetail;
using Todo_App.Domain.Enums;
using Todo_App.Domain.ValueObjects;
namespace Todo_App.Application.UnitTests.TodoItem.UpdateTodoItemDetail;
public class UpdateTodoItemDetailCommandHandlerTest
{
    [Test]
    public async Task ShouldUpdateItemAndSaveChanges()
    {
        // ARRANGE 
        var itemId = 1;
        var listId = 1;
        var priority = PriorityLevel.High;
        var note = "Te";
        var color = "#FF5733";
        var cancellationToken = CancellationToken.None;

        var mockDbContext = new Mock<IApplicationDbContext>();
        var mockDbSet = new Mock<DbSet<Domain.Entities.TodoItem>>();

        mockDbContext.Setup(m => m.TodoItems).Returns(mockDbSet.Object);
        var updateCommandHandler = new UpdateTodoItemDetailCommandHandler(mockDbContext.Object);

        var testEntity = new Domain.Entities.TodoItem
        {
            Id = itemId,
            ListId = listId,
            Priority = priority,
            Note = note,
            Colour = Colour.From(color)
        };
        mockDbSet.Setup(m => m.FindAsync(new object[] { itemId }, cancellationToken)).ReturnsAsync(testEntity);

        var updateCommand = new UpdateTodoItemDetailCommand
        {
            Id = itemId,
            ListId = listId,
            Priority = PriorityLevel.Low, 
            Note = "Updated Note", 
            Color = "#FF5733"
        };

        // ACT
        await updateCommandHandler.Handle(updateCommand, cancellationToken);

        // ASSERT
        Assert.AreEqual(PriorityLevel.Low, testEntity.Priority);
        Assert.AreEqual("Updated Note", testEntity.Note);
        Assert.AreEqual(color, testEntity.Colour.ToString());
        mockDbContext.Verify(m => m.SaveChangesAsync(cancellationToken), Times.Once);
    }
}

